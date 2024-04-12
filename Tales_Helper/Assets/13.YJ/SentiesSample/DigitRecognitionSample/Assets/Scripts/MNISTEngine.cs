using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Sentis;
using System.Linq;

/*
 *  Neural net engine and handles the inference.
 *   - Shifts the image to the center for better inference. 
 *   (The model was trained on images centered in the texture this will give better results)
 *  - recentering of the image is also done using special operations on the GPU
 */

// 현재 그려진 이미지의 경계 상태입니다. 이미지를 나중에 다시 중앙으로 재설정해야 할 경우 도움이 됩니다.
public struct Bounds
{
    public int left;
    public int right;
    public int top;
    public int bottom;
}

public class MNISTEngine : MonoBehaviour
{
    public ModelAsset mnistONNX; // MNIST 모델에 대한 에셋

    // 엔진 타입
    IWorker engine;

    // 이 작은 모델은 CPU 및 GPU 모두에서 빠르게 작동합니다.
    static Unity.Sentis.BackendType backendType = Unity.Sentis.BackendType.GPUCompute; // 백엔드 타입은 GPU Compute로 설정

    // 이미지의 너비와 높이:
    const int imageWidth = 28; // 이미지의 너비와 높이를 28로 설정

    // 입력 텐서
    TensorFloat inputTensor = null;

    // Tensor를 조작하는 연산
    Ops ops;

    Camera lookCamera; // 이미지에서 광선을 계산하는 데 사용할 카메라

    void Start()
    {
        // 에셋에서 신경망 모델을 로드합니다.
        Model model = ModelLoader.Load(mnistONNX);
        // 신경망 엔진을 생성합니다.
        engine = WorkerFactory.CreateWorker(backendType, model);

        // Ops를 사용하여 텐서에 직접 연산을 수행할 수 있습니다.
        ops = WorkerFactory.CreateOps(backendType, null);

        // 이미지에서 광선을 계산할 카메라를 가져옵니다.
        lookCamera = Camera.main;
    }

    // 이미지를 신경망 모델로 보내고 각 숫자일 확률을 반환합니다.
    public (float, int) GetMostLikelyDigitProbability(Texture2D drawableTexture)
    {
        inputTensor?.Dispose();

        // 텍스처를 텐서로 변환합니다. 너비=W, 높이=W, 채널=1입니다.
        inputTensor = TextureConverter.ToTensor(drawableTexture, imageWidth, imageWidth, 1);

        // 신경망을 실행합니다.
        engine.Execute(inputTensor);

        // GPU에 결과를 유지한 채 신경망의 출력을 얻습니다.
        TensorFloat result = engine.PeekOutput() as TensorFloat;

        // 소프트맥스 함수를 사용하여 결과를 0에서 1 사이의 확률로 변환합니다.
        var probabilities = ops.Softmax(result);
        var indexOfMaxProba = ops.ArgMax(probabilities, -1, false);

        // GPU에서 결과를 CPU에서 읽을 수 있도록 만듭니다.
        probabilities.MakeReadable();
        indexOfMaxProba.MakeReadable();

        var predictedNumber = indexOfMaxProba[0];
        var probability = probabilities[predictedNumber];

        return (probability, predictedNumber);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseClicked();
        }
        else if (Input.GetMouseButton(0))
        {
            MouseIsDown();
        }
    }

    // 마우스 클릭을 감지하고 정보를 패널 클래스로 전송합니다.
    void MouseClicked()
    {
        Ray ray = lookCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.name == "Screen")
        {
            Panel panel = hit.collider.GetComponentInParent<Panel>();
            if (!panel) return;
            panel.ScreenMouseDown(hit);
        }
    }

    // 마우스가 눌려 있는지 감지하고 정보를 패널 클래스로 전송합니다.
    void MouseIsDown()
    {
        Ray ray = lookCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.name == "Screen")
        {
            Panel panel = hit.collider.GetComponentInParent<Panel>();
            if (!panel) return;
            panel.ScreenGetMouse(hit);
        }
    }

    // 세션 종료시 모든 리소스를 정리하여 GPU 또는 메모리에 남아있지 않도록 합니다.
    private void OnDestroy()
    {
        inputTensor?.Dispose();
        engine?.Dispose();
        ops?.Dispose();
    }

}
