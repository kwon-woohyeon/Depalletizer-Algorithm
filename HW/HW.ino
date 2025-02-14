#include <Arduino_FreeRTOS.h>
#include <Servo.h>

// 핀 정의
#define TRIG1_PIN 22  // 초음파1 Trig 핀
#define ECHO1_PIN 23  // 초음파1 Echo 핀
#define TRIG2_PIN 24  // 초음파2 Trig 핀
#define ECHO2_PIN 25  // 초음파2 Echo 핀
#define IN1 9
#define IN2 10
#define IN3 11
#define IN4 12

// 6개의 Servo 객체 생성
Servo servo1;
Servo servo2;
Servo servo3;
Servo servo4;
Servo servo5;
Servo servo6;

// 전역 변수 선언
volatile int repeatCount = 0;  // 현재 반복 횟수
volatile int maxCount = 0;     // 최대 반복 횟수, 초기값 0 (사용자 입력 대기)

// 전역 플래그 선언
volatile bool isRunning = false;  // 시스템 동작 상태 (초기 상태: 멈춤)

const int maxCommands = 10;      // 저장할 명령어의 최대 개수
char commandQueue[maxCommands];  // 명령어를 저장할 배열
int commandCount = 0;            // 현재 저장된 명령어 개수

// 태스크 핸들러 선언
void TaskMotorB(void *pvParameters);
void TaskMotorA(void *pvParameters);
void TaskSerialInput(void *pvParameters);  // 키 입력 처리 태스크 추가
void TaskRobotArm(void *pvParameters);     // 로봇팔 태스크 함수 선언
void TaskSerialOutput(void *pvParameters);
void TaskDistanceCapture(void *pvParameters);

bool isMotorActive = false;  // 모터 B 동작 상태

// 함수 프로토타입 선언
float measureDistance();
unsigned long measureEchoTime();
float measureDistance2();
unsigned long measureEchoTime2();
void position0();
void position1();
void position2();
void position3();
void position4();
void position5();
void stopAtPosition0();
void forcePosition0();  // 강제로 초기 위치로 이동하는 함수
void sendESPCommand(String command);
void sendDataToClient(String data);
void sendCompletionMessage();

void setup() {
  Serial.begin(9600);
  Serial.println("시스템 초기화 중...");

  // ESP-01 Serial1 초기화 (핀 19: RX, 핀 18: TX)
  Serial1.begin(9600);

  // 로봇팔 강제 초기화
  forcePosition0();  // 초기 위치 강제 설정

  // 모터 핀 설정
  pinMode(IN1, OUTPUT);
  pinMode(IN2, OUTPUT);
  pinMode(IN3, OUTPUT);
  pinMode(IN4, OUTPUT);

  // 초기 상태 설정
  digitalWrite(IN1, LOW);
  digitalWrite(IN2, LOW);
  digitalWrite(IN3, LOW);
  digitalWrite(IN4, LOW);

  // 초음파 핀 설정
  pinMode(TRIG1_PIN, OUTPUT);
  pinMode(ECHO1_PIN, INPUT);

  pinMode(TRIG2_PIN, OUTPUT);
  pinMode(ECHO2_PIN, INPUT);


  Serial.println("로봇팔 초기화 완료");
  Serial.println("멀티태스킹 시작");
  Serial.println("최대 반복 횟수를 입력하세요:");

  // FreeRTOS 태스크 생성
  xTaskCreate(TaskMotorB, "Motor B Task", 256, NULL, 1, NULL);
  xTaskCreate(TaskMotorA, "Motor A Task", 256, NULL, 1, NULL);
  xTaskCreate(TaskRobotArm, "Robot Arm Task", 256, NULL, 1, NULL);
  xTaskCreate(TaskSerialInput, "Serial Input Task", 256, NULL, 1, NULL);
  xTaskCreate(TaskSerialOutput, "Serial Output Task", 256, NULL, 1, NULL);
  xTaskCreate(TaskDistanceCapture, "Distance Capture Task", 256, NULL, 1, NULL);

  vTaskStartScheduler();

}

void loop() {
  // FreeRTOS가 동작 중일 때 loop()는 사용되지 않음
}

// 강제로 초기 위치로 이동하는 함수
void forcePosition0() {
  Serial.println("로봇팔 강제로 초기 위치로 이동 중...");

  // 서보 강제 위치 설정
  servo1.attach(2);
  servo1.write(100);

  servo2.attach(3);
  servo2.write(90);

  servo3.attach(4);
  servo3.write(170);

  servo4.attach(5);
  servo4.write(110);

  servo5.attach(6);
  servo5.write(178);

  servo6.attach(7);
  servo6.write(0);

  Serial.println("강제 초기 위치 설정 완료");
}

//데이터 송신 함수
void sendCompletionMessage() {
  String message = "complete";
  Serial.println("송신: " + message);
  sendDataToClient(message);
}

void TaskSerialOutput(void *pvParameters) {
  while (1) {
    if (Serial1.available()) {
      String response = Serial1.readStringUntil('\n');
      response.trim();
      Serial.println("ESP-01 수신: " + response);

      // +IPD 메시지 처리
      if (response.startsWith("+IPD")) {
        int colonIndex = response.indexOf(':');  // ':' 위치 찾기
        if (colonIndex != -1) {
          String actualData = response.substring(colonIndex + 1);  // ':' 이후 데이터 추출
          Serial.println("실제 데이터: " + actualData);

          // 실제 데이터 처리
          if (actualData.equals("start")) {
            isRunning = true;
            if(maxCount==0)
            {
              sendCompletionMessage();
            }
            Serial.println("시스템 시작 명령 수신: 태스크 활성화");
          } else if (actualData.equals("stop")) {
            isRunning = false;
            Serial.println("시스템 종료 명령 수신: 태스크 비활성화");
          } else if (actualData.length() == 1) {  // 단일 문자 명령 처리
            char command = actualData.charAt(0);
            if (command == 'P' || command == 'N') {
              if (commandCount < maxCommands) {  // 배열에 여유 공간이 있으면 추가
                commandQueue[commandCount++] = command;
                Serial.print("명령 추가됨: ");

                // 배열 전체 출력
                for (int i = 0; i < commandCount; i++) {
                  Serial.print(commandQueue[i]);
                  Serial.print(" ");
                }
                Serial.println();
              } else {
                Serial.println("명령 큐가 가득 찼습니다.");
              }
            } else if (isdigit(command)) {  // 숫자인 경우 maxCount 설정
              maxCount = command - '0';  // 문자에서 숫자로 변환
              Serial.println("최대 반복 횟수 설정: " + String(maxCount));
            } else {
              Serial.println("알 수 없는 단일 문자 명령: " + actualData);
            }
          } else {
            Serial.println("알 수 없는 명령 수신: " + actualData);
          }
        } else {
          Serial.println("올바르지 않은 IPD 메시지: " + response);
        }
      }
      // 기존 메시지 처리 (IPD가 아닌 경우)
      else {
        if (response.equals("start")) {
          isRunning = true;
          Serial.println("시스템 시작 명령 수신: 태스크 활성화");
        } else if (response.equals("stop")) {
          isRunning = false;
          Serial.println("시스템 종료 명령 수신: 태스크 비활성화");
        } else {
          int startIndex = response.indexOf(':');  // ':' 위치 찾기
          if (startIndex != -1 && startIndex + 1 < response.length()) {
            char command = response.charAt(startIndex + 1);  // ':' 이후 문자 추출
            if (isdigit(command)) {                  // 숫자인지 확인
              maxCount = command - '0';                     // 문자에서 숫자로 변환
            } else if (command == 'P' || command == 'N') {  // 'P' 또는 'N' 명령 확인
              if (commandCount < maxCommands) {             // 배열에 여유 공간이 있으면 추가
                commandQueue[commandCount++] = command;
                Serial.print("명령 추가됨: ");

                // 배열 전체 출력
                for (int i = 0; i < commandCount; i++) {
                  Serial.print(commandQueue[i]);
                  Serial.print(" ");
                }
                Serial.println();
              } else {
                Serial.println("명령 큐가 가득 찼습니다.");
              }
            }
          } else {
            Serial.println("알 수 없는 명령 수신: " + response);
          }
        }
      }
    }

    vTaskDelay(100 / portTICK_PERIOD_MS);  // 100ms 대기
  }
}




// 명령 전송 함수
void sendESPCommand(String command) {
  while (Serial1.available()) {  // ESP-01이 현재 처리 중인지 확인
    String busyCheck = Serial1.readStringUntil('\n');
    if (busyCheck.indexOf("busy") != -1) {
      delay(100);  // 바쁠 경우 대기
    }
  }
  Serial1.println(command + "\r\n");  // 명령 전송 (줄 바꿈 포함)
}

// C# 클라이언트로 데이터 전송 함수
void sendDataToClient(String data) {
  String command = "AT+CIPSEND=0," + String(data.length());  // 클라이언트 ID: 0, 데이터 길이 지정
  sendESPCommand(command);                                   // 전송 명령 전송
  delay(200);
  Serial1.print(data);  // 실제 데이터 전송
  delay(200);
  Serial.println("데이터 전송 완료: " + data);
}

// Serial Monitor에서 명령어 처리
void TaskSerialInput(void *pvParameters) {
  while (1) {
    if (Serial.available() > 0) {
      String input = Serial.readStringUntil('\n');
      input.trim();

      // ESP-01에 데이터 전송
      if (input.startsWith("AT")) {
        sendESPCommand(input);
      } else {
        // 일반 데이터 전송
        sendDataToClient(input);
        Serial.println("전송된 데이터: " + input);
      }
    }
    vTaskDelay(10 / portTICK_PERIOD_MS);  // 10ms 대기
  }
}

// 초음파2 거리 측정 함수
float measureDistance2() {
  long duration;
  float distance;

  digitalWrite(TRIG2_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG2_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG2_PIN, LOW);

  duration = measureEchoTime2();

  if (duration == 0) {
    return -1;
  }

  distance = duration * 0.034 / 2;
  if (distance > 400) {
    return -1;
  }

  return distance;
}

// 비블로킹 방식의 Echo2 신호 측정
unsigned long measureEchoTime2() {
  unsigned long startTime = micros();

  while (digitalRead(ECHO2_PIN) == LOW) {
    if (micros() - startTime > 100000) return 0;  // 타임아웃
  }

  startTime = micros();

  while (digitalRead(ECHO2_PIN) == HIGH) {
    if (micros() - startTime > 100000) return 0;  // 타임아웃
  }

  return micros() - startTime;
}

// 초음파2 거리 기반 캡처 태스크
void TaskDistanceCapture(void *pvParameters) {
  while (1) {
    float distance = measureDistance2(); // 초음파2 거리 측정
    String data = "capture";

    if (distance > 0 && distance <= 10) {  // 특정 거리 조건
      sendDataToClient(data); // 데이터 전송
      Serial.println("거리 조건 충족: capture 메시지 전송");
      vTaskDelay(1000 / portTICK_PERIOD_MS);  // 1초 대기
    }
    vTaskDelay(1000 / portTICK_PERIOD_MS);  // 100ms 대기
  }
}


// 초음파 거리 측정 함수
float measureDistance() {
  long duration;
  float distance;

  digitalWrite(TRIG1_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG1_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG1_PIN, LOW);

  duration = measureEchoTime();

  if (duration == 0) {
    return -1;
  }

  distance = duration * 0.034 / 2;
  if (distance > 400) {
    return -1;
  }
  return distance;
}

// 비블로킹 방식의 Echo 신호 측정
unsigned long measureEchoTime() {
  unsigned long startTime = micros();

  // Echo 핀이 HIGH로 변할 때까지 대기
  while (digitalRead(ECHO1_PIN) == LOW) {
    if (micros() - startTime > 100000) return 0;  // 타임아웃
  }

  startTime = micros();

  // Echo 핀이 LOW로 변할 때까지 대기
  while (digitalRead(ECHO1_PIN) == HIGH) {
    if (micros() - startTime > 100000) return 0;  // 타임아웃
  }

  return micros() - startTime;
}

// 0번 위치 동작 함수
void position0() {
  servo1.write(100);
  servo6.write(0);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo2.write(90);
  servo3.write(170);
  servo4.write(110);
  servo5.write(178);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
}

// 1번 위치 동작 함수
void position1() {
  servo4.write(134);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo1.write(143);
  vTaskDelay(500 / portTICK_PERIOD_MS);
  servo4.write(110);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo6.write(89);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo3.write(160);
  servo4.write(133);
  vTaskDelay(2000 / portTICK_PERIOD_MS);
  servo1.write(100);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo4.write(120);
}

// 2번 위치 동작 함수
void position2() {
  servo4.write(134);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo1.write(143);
  vTaskDelay(500 / portTICK_PERIOD_MS);
  servo4.write(110);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo6.write(109);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo2.write(96);
  servo3.write(160);
  servo4.write(133);
  vTaskDelay(2000 / portTICK_PERIOD_MS);
  servo1.write(100);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo4.write(120);
}

// 3번 위치 동작 함수
void position3() {
  servo4.write(134);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo1.write(143);
  vTaskDelay(500 / portTICK_PERIOD_MS);
  servo4.write(110);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo6.write(89);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo3.write(131);
  vTaskDelay(100 / portTICK_PERIOD_MS);
  servo5.write(171);
  vTaskDelay(100 / portTICK_PERIOD_MS);
  servo4.write(142);
  servo2.write(87);
  vTaskDelay(2000 / portTICK_PERIOD_MS);
  servo1.write(100);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo4.write(120);
  servo3.write(155);
}

// 4번 위치 동작 함수
void position4() {
  servo4.write(134);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo1.write(143);
  vTaskDelay(500 / portTICK_PERIOD_MS);
  servo4.write(110);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo6.write(103);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo2.write(100);
  servo3.write(130);
  vTaskDelay(100 / portTICK_PERIOD_MS);
  servo5.write(171);
  vTaskDelay(100 / portTICK_PERIOD_MS);
  servo4.write(144);
  vTaskDelay(2000 / portTICK_PERIOD_MS);
  servo1.write(100);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo4.write(120);
  servo3.write(155);
}

// 5번 위치 동작 함수
void position5() {
  servo4.write(134);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo1.write(143);
  vTaskDelay(500 / portTICK_PERIOD_MS);
  servo4.write(110);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo6.write(180);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo4.write(130);
  vTaskDelay(2000 / portTICK_PERIOD_MS);
  servo1.write(100);
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  servo4.write(120);
}

// 0번 위치에서 멈추는 함수
void stopAtPosition0() {
  position0();
  while (maxCount == 0) {
    vTaskDelay(100 / portTICK_PERIOD_MS);
  }
}

// 모터 A 동작 태스크
void TaskMotorA(void *pvParameters) {
  while (1) {
    if (isRunning) {
      analogWrite(IN1, 90);  // PWM 신호로 모터 속도 제어
      digitalWrite(IN2, LOW); // 모터 방향 설정
    } else {
      // 멈춘 상태
      analogWrite(IN1, 0);   // PWM 신호를 0으로 설정하여 정지
      digitalWrite(IN2, LOW);
    }

    vTaskDelay(100 / portTICK_PERIOD_MS);  // 100ms 대기
  }
}


// 모터 B 제어 태스크
void TaskMotorB(void *pvParameters) {
  while (1) {
    if (!isRunning || !isMotorActive) {
      vTaskDelay(50 / portTICK_PERIOD_MS);
      continue;
    }
    if (isRunning && isMotorActive) {
      Serial.println("모터 B 반대 방향 동작 시작 (3초)");
      analogWrite(IN4, 255);   // 반대 방향
      digitalWrite(IN3, LOW);  // 기존 방향 끔
      vTaskDelay(2360 / portTICK_PERIOD_MS);
      analogWrite(IN4, 0);  // 정지
      digitalWrite(IN3, LOW);
      isMotorActive = false;
      Serial.println("모터 B 정지");
    }
    vTaskDelay(50 / portTICK_PERIOD_MS);
  }
}

void TaskRobotArm(void *pvParameters) {
  // 포인터 배열을 사용하여 position1 ~ position4 함수들을 저장
  void (*positions[])(void) = { position1, position2, position3, position4 };
  const int totalPositions = sizeof(positions) / sizeof(positions[0]);

  int positionIndex = 0;  // 현재 동작할 position 배열의 인덱스

  while (1) {
    // 실행 상태가 아닐 경우 대기
    while (!isRunning) {
      vTaskDelay(100 / portTICK_PERIOD_MS);  // 100ms 대기
    }

    // 초음파1 거리 측정
    float distance = measureDistance();

    if (distance < 9.5) {  // 초음파1 조건 충족 확인
      
      // 명령이 있는 경우 동작
      if (commandCount > 0) {
        // 큐에서 현재 명령 확인
        char command = commandQueue[0];

        if (command == 'N') {
          position0();
          position5();  // N 명령에 따라 position5 동작
        } else if (command == 'P') {
          position0();                     // 공통 동작: position0 호출
          positions[positionIndex % 4]();  // 현재 포지션 호출
          positionIndex++;                 // P 명령에서만 positionIndex 증가
          repeatCount++;                   // P인 경우 반복 횟수 증가

          // repeatCount가 4에 도달하면 모터 B 활성화
          if (repeatCount == 4) {
            isMotorActive = true;  // 모터 B 동작 활성화
            Serial.println("모터 B 활성화: repeatCount = 4");
          }
        }

        // 명령 실행 후 큐에서 제거
        for (int j = 0; j < commandCount - 1; j++) {
          commandQueue[j] = commandQueue[j + 1];
        }
        commandCount--;

        // 남아있는 명령 출력
        Serial.print("남아있는 명령: ");
        for (int k = 0; k < commandCount; k++) {
          Serial.print(commandQueue[k]);
          Serial.print(" ");
        }
        Serial.println();

        // maxCount를 초과했는지 확인
        if (repeatCount >= maxCount) {
          stopAtPosition0();     // 로봇팔 정지
          positionIndex = 0;     // position 순서 초기화
          isMotorActive = true;  // 모터 B 동작 활성화
          Serial.println("모터 B 활성화: repeatCount = maxCount");
          sendCompletionMessage();  // 작업 완료 메시지 송신
          Serial.println("작업 완료 후 대기 중...");
          repeatCount = 0;       // repeatCount 초기화
          maxCount = 0;          // maxCount 초기화
        }
      }
    }
    vTaskDelay(100 / portTICK_PERIOD_MS);  // 100ms 대기
  }
}