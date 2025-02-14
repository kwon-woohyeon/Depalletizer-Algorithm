using Clipper2Lib;
using Compunet.YoloSharp;  // YOLO 모델을 사용하는 라이브러리
using OpenCvSharp;         // OpenCV 라이브러리
using OpenCvSharp.Extensions; // Bitmap -> Mat 변환을 위한 확장 메서드
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing; // Bitmap 사용
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Compunet.YoloSharp.Data;
using System.Text;

namespace Project
{
    public class VideoProcessor
    {
        private Form1 _form1;  // Form1의 참조 변수
        private string connectionString = "Server=localhost;Database=project;User ID=root;Password=1234;";
        public VideoCapture _capture;  // 웹캠 비디오 캡처
        public YoloPredictor _predictor; // YOLO 모델 예측기
        public Mat _maskImage; // 마스크 이미지
        public string capture_sign; // 캡처 신호
        public bool _isRunning = false; // 비디오 처리 중인지 여부를 추적하는 플래그
        public bool state = false;
        private database db;
        public int realid = 0;
        public string complete_1;


        public VideoProcessor(Form1 form1)
        {

            _form1 = form1; // Form1 인스턴스 저장
            string filePath = @"C:\Users\user\Desktop\Project\onnx\error.onnx"; // 실제 YOLO 모델 경로
            _predictor = new YoloPredictor(filePath);
            db = new database(connectionString);

            // 리소스에서 Bitmap 로드 후 OpenCV Mat으로 변환
            using (var bitmap = new Bitmap(Properties.Resources.mask))
            {
                _maskImage = BitmapConverter.ToMat(bitmap);
            }

            _capture = new VideoCapture(0); // 웹캠 초기화
            if (!_capture.IsOpened())
            {
                throw new Exception("웹캠을 열 수 없습니다.");
            }
        }

        public async Task ProcessVideoAsync() // 비동기 영상 처리 메서드
        {

            try
            {
                int id = db.GetFirstIdForToday();
                int count = db.GetstateForId(id);
                string total = db.GetTotalForId(id);
                int count2 = db.GetNGForId(id);
                int statecount = 0;
                while (_isRunning) // 비디오가 처리 중일 때
                {
                    if (complete_1 == "complete")
                    {

                        int x = db.Get_Total_Today_NOW();
                        byte[] dataToSend = Encoding.UTF8.GetBytes(x.ToString());
                        await _form1.stream.WriteAsync(dataToSend, 0, dataToSend.Length); // 데이터 전송
                        complete_1 = "";
                        //MessageBox.Show(x.ToString());
                    }
                    var stopwatch = Stopwatch.StartNew(); // 처리 시간 측정을 위한 Stopwatch 시작

                    // 웹캠에서 프레임 읽기
                    using var frame = new Mat();
                    _capture.Read(frame); // 웹캠에서 한 프레임을 읽어옴
                    if (frame.Empty())
                    {
                        throw new Exception("웹캠에서 프레임을 읽을 수 없습니다.");
                    }

                    // 프레임 리사이즈
                    using var resizedFrame = new Mat();
                    Cv2.Resize(frame, resizedFrame, new OpenCvSharp.Size(640, 640));

                    // 마스크 적용
                    using var bitwiseFrame = new Mat();
                    if (_maskImage != null)
                    {
                        using var resizedMask = new Mat();
                        Cv2.Resize(_maskImage, resizedMask, new OpenCvSharp.Size(resizedFrame.Width, resizedFrame.Height));

                        // resizedMask가 CV_8UC4(알파 채널 포함)인 경우 CV_8UC3(알파 채널 제거)로 변환
                        if (resizedMask.Type() == MatType.CV_8UC4)
                        {
                            Cv2.CvtColor(resizedMask, resizedMask, ColorConversionCodes.BGRA2BGR);
                        }

                        // resizedMask가 그레이스케일(CV_8UC1)인 경우 컬러(CV_8UC3)로 변환
                        if (resizedMask.Channels() == 1)
                        {
                            Cv2.CvtColor(resizedMask, resizedMask, ColorConversionCodes.GRAY2BGR);
                        }

                        // BitwiseAnd 수행
                        if (resizedFrame.Size() == resizedMask.Size() && resizedFrame.Type() == resizedMask.Type())
                        {
                            Cv2.BitwiseAnd(resizedFrame, resizedMask, bitwiseFrame);
                        }
                        else
                        {
                            throw new Exception($"크기 또는 타입이 일치하지 않습니다. resizedFrame: {resizedFrame.Size()}, {resizedFrame.Type()}, resizedMask: {resizedMask.Size()}, {resizedMask.Type()}");
                        }
                    }
                    else
                    {
                        resizedFrame.CopyTo(bitwiseFrame); // 마스크가 없으면 원본을 그대로 사용
                    }

                    // 관심 영역(ROI) 설정
                    int roiX = 0, roiY = 130, roiWidth = 640, roiHeight = 380;
                    Rect roi = new Rect(roiX, roiY, roiWidth, roiHeight);
                    using var roiFrame = new Mat(bitwiseFrame, roi);

                    // 그레이스케일 변환 및 이진화
                    using var grayFrame = new Mat();
                    Cv2.CvtColor(bitwiseFrame, grayFrame, ColorConversionCodes.BGR2GRAY);
                    using var binaryFrame = new Mat();
                    Cv2.Threshold(grayFrame, binaryFrame, 80, 255, ThresholdTypes.Binary);

                    // 컨투어 검출
                    Cv2.FindContours(binaryFrame, out OpenCvSharp.Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    // 컨투어 그리기 및 바운딩 박스 생성
                    using var contourFrame = new Mat(binaryFrame.Size(), MatType.CV_8UC3, Scalar.All(0));
                    var rectangles = new List<Rect>();
                    var resultsPerIndex = new Dictionary<int, List<string>>(); // 인덱스별로 결과를 저장할 딕셔너리

                    foreach (var contour in contours)
                    {
                        double area = Cv2.ContourArea(contour);
                        if (area >= 30000)
                        {
                            Cv2.DrawContours(contourFrame, new[] { contour }, -1, new Scalar(0, 255, 0), 2);
                            var rect = Cv2.BoundingRect(contour);
                            rectangles.Add(rect);
                            Cv2.Rectangle(bitwiseFrame, rect, new Scalar(255, 0, 0), 2);
                        }
                    }

                    // x축 기준으로 직사각형을 정렬하고 인덱스를 부여
                    rectangles.Sort((a, b) => b.X.CompareTo(a.X)); // X 좌표 기준 내림차순 정렬
                    int index = 1;

                    foreach (var rect in rectangles) // 각 직사각형에 대해
                    {
                        Cv2.PutText(bitwiseFrame, index.ToString(), new OpenCvSharp.Point(rect.X, rect.Y - 5), HersheyFonts.HersheySimplex, 0.6, new Scalar(0, 255, 255), 2); // 인덱스 텍스트 추가
                        index++;
                    }

                    _form1.UpdateLiveImage(roiFrame);

                    // YOLO 탐지
                    if (capture_sign == "capture")
                    {


                        byte[] imageData;
                        Cv2.ImEncode(".jpg", bitwiseFrame, out imageData); // Mat을 .jpg 형식으로 인코딩 후 바이트 배열 반환

                        var detections = await Task.Run(() => _predictor.Detect(imageData)); // YOLO 모델로 객체 검출
                        state = false;
                        // 기존 직사각형을 확인하고, 해당 직사각형에 대해 결과를 저장하는 부분
                        if (detections.Count == 0)
                        {
                            // 객체가 검출되지 않았을 때의 로직 처리
                            foreach (var rect in rectangles)
                            {
                                var rectIndex = rectangles.IndexOf(rect);

                                if (!resultsPerIndex.ContainsKey(rectIndex))
                                {
                                    resultsPerIndex[rectIndex] = new List<string>();
                                }
                                resultsPerIndex[rectIndex] = new List<string> { "OK" };  // "OK"로 처리
                            }
                        }

                        foreach (var rect in rectangles) // 각 직사각형에 대해
                        {
                            bool isNG = false; // NG 여부를 추적하는 플래그

                            foreach (var detection in detections)
                            {
                                var detectionRect = new Rect(detection.Bounds.X, detection.Bounds.Y, detection.Bounds.Width, detection.Bounds.Height);
                                Cv2.Rectangle(bitwiseFrame, detectionRect, new Scalar(0, 255, 0), 2); // 탐지된 객체 사각형을 그리기
                                Cv2.PutText(bitwiseFrame, $"{detection.Name} ({detection.Confidence * 100:F2}%)",
                                            new OpenCvSharp.Point(detection.Bounds.X, detection.Bounds.Y - 5),
                                            HersheyFonts.HersheySimplex, 0.6, new Scalar(0, 255, 0), 2);

                                // 직사각형이 YOLO 탐지 객체와 일부분이라도 겹치는지 확인
                                if (rect.IntersectsWith(detectionRect))
                                {
                                    isNG = true; // 겹친다면 NG
                                    break; // 하나라도 겹치면 더 이상 확인할 필요 없음
                                }
                            }

                            // 결과를 저장
                            var rectIndex = rectangles.IndexOf(rect);
                            if (!resultsPerIndex.ContainsKey(rectIndex))
                            {
                                resultsPerIndex[rectIndex] = new List<string>();
                            }

                            // NG인 경우
                            if (isNG)
                            {
                                resultsPerIndex[rectIndex] = new List<string> { "NG" };
                            }
                            else
                            {
                                // 겹치지 않으면 OK
                                resultsPerIndex[rectIndex] = new List<string> { "OK" };
                            }
                        }





                        stopwatch.Stop(); // 시간 측정 종료
                        Cv2.PutText(roiFrame, $"Time: {stopwatch.ElapsedMilliseconds} ms", new OpenCvSharp.Point(10, 30), HersheyFonts.HersheySimplex, 0.6, new Scalar(0, 255, 255), 2);




                        // bitwiseFrame에서 ROI 추출
                        Rect bitwiseFrame2_roi = new Rect(roiX, roiY, roiWidth, roiHeight);
                        using var bitwiseFrame2_roiFrame = new Mat(bitwiseFrame, bitwiseFrame2_roi); // 관심 영역만 따로 추출

                        _form1.statelabel.Text = "";
                        // 상태 업데이트
                        foreach (var kvp in resultsPerIndex)
                        {
                            if (kvp.Value.Contains("NG"))
                            {
                                _form1.statelabel.Text += "NG";


                                byte[] dataToSend = Encoding.UTF8.GetBytes("N");
                                await _form1.stream.WriteAsync(dataToSend, 0, dataToSend.Length); // 데이터 전송


                                count2++;
                                statecount = 1;
                                if (total == count.ToString())
                                {
                                    try
                                    {
                                        realid = id;
                                        // 데이터베이스에 값 추가
                                        db.UpdateStateForFirstIdAndGetTotal(id, count.ToString());
                                        id = db.GetNextIdForToday(id);
                                        total = db.GetTotalForId(id);


                                        count2 = db.GetNGForId(id);
                                        count = db.GetstateForId(id);

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"오류 발생333: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        realid = id;
                                        // 데이터베이스에 값 추가
                                        db.UpdateStateForFirstIdAndGetTotal(id, count.ToString());
                                        db.UpdateNG(id, count2.ToString());

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"오류 발생222: {ex.Message}");
                                    }
                                }

                                _form1.SaveCapturedImage(bitwiseFrame2_roiFrame, statecount, kvp.Key + 1);
                            }
                            if (kvp.Value.Contains("OK"))
                            {
                                _form1.statelabel.Text += "OK";

                                _form1.state_flag = "P";
                                byte[] dataToSend = Encoding.UTF8.GetBytes("P");
                                await _form1.stream.WriteAsync(dataToSend, 0, dataToSend.Length); // 데이터 전송

                                count++;
                                statecount = 2;

                                if (total == count.ToString())
                                {
                                    try
                                    {
                                        realid = id;
                                        // 데이터베이스에 값 추가
                                        db.UpdateStateForFirstIdAndGetTotal(id, count.ToString());
                                        id = db.GetNextIdForToday(id);
                                        total = db.GetTotalForId(id);


                                        count2 = db.GetNGForId(id);
                                        count = db.GetstateForId(id);

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"오류 발생333: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        realid = id;
                                        // 데이터베이스에 값 추가
                                        db.UpdateStateForFirstIdAndGetTotal(id, count.ToString());
                                        db.UpdateNG(id, count2.ToString());

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"오류 발생222: {ex.Message}");
                                    }
                                }

                                _form1.SaveCapturedImage(bitwiseFrame2_roiFrame, statecount, kvp.Key + 1);
                            }
                        }



                        _form1.UpdatePictureBox2(bitwiseFrame2_roiFrame);

                        capture_sign = " ";
                    }
                    await Task.Delay(33); // 프레임 처리 간 약간의 딜레이 (30 FPS로 처리)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}"); // 오류 발생 시 메시지 박스 표시
            }
        }
    }
}