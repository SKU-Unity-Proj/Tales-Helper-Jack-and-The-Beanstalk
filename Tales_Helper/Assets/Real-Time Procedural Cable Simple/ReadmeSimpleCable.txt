Procedural Cable Simple v1.1
DrinkingWindGames



NOTE:  All you need from the downloaded asset package is "CableProceduralSimple"




사용 방법:

"CableProceduralSimple" 컴포넌트를 빈(empty) 오브젝트에 추가하면 자동으로 LineRenderer가 추가됩니다.

Inspector에서 endPointTransform에 원하는 다른 Transform을 할당하세요. (다른 빈(empty) 오브젝트 사용 권장)

LineRenderer를 원하는대로 설정해야 합니다. 포지션에는 아무것도 할 필요가 없습니다.

CableProceduralSimple의 값을 원하는대로 설정하세요:

Point Density: 케이블을 정의하는 단위 길이당 포인트 수 (낮은 밀도는 높은 성능을 의미).
Sag Amplitude: 중간에서 케이블이 얼마나 휘어질지를 Unity 단위(미터)로 설정.
Sway Multiplier: 케이블이 좌우 및 상하로 얼마나 흔들릴지를 Unity 단위로 설정.
Sway X Multiplier: 케이블이 로컬 X 방향으로 얼마나 흔들릴지를 변경합니다.
Sway Y Multiplier: 케이블이 로컬 Y 방향으로 얼마나 흔들릴지를 변경합니다.
Sway Frequency: 케이블이 초당 몇 번 주기적으로 움직일지를 설정합니다 (헤르츠).
팁:

체인의 경우 lineRenderer의 "텍스처 모드"를 "각 세그먼트마다 반복"으로 설정하세요.