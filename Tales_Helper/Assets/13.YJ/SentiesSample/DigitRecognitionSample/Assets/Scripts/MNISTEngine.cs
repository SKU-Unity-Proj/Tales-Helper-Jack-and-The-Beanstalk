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

// ���� �׷��� �̹����� ��� �����Դϴ�. �̹����� ���߿� �ٽ� �߾����� �缳���ؾ� �� ��� ������ �˴ϴ�.
public struct Bounds
{
    public int left;
    public int right;
    public int top;
    public int bottom;
}

public class MNISTEngine : MonoBehaviour
{
    public ModelAsset mnistONNX; // MNIST �𵨿� ���� ����

    // ���� Ÿ��
    IWorker engine;

    // �� ���� ���� CPU �� GPU ��ο��� ������ �۵��մϴ�.
    static Unity.Sentis.BackendType backendType = Unity.Sentis.BackendType.GPUCompute; // �鿣�� Ÿ���� GPU Compute�� ����

    // �̹����� �ʺ�� ����:
    const int imageWidth = 28; // �̹����� �ʺ�� ���̸� 28�� ����

    // �Է� �ټ�
    TensorFloat inputTensor = null;

    // Tensor�� �����ϴ� ����
    Ops ops;

    Camera lookCamera; // �̹������� ������ ����ϴ� �� ����� ī�޶�

    void Start()
    {
        // ���¿��� �Ű�� ���� �ε��մϴ�.
        Model model = ModelLoader.Load(mnistONNX);
        // �Ű�� ������ �����մϴ�.
        engine = WorkerFactory.CreateWorker(backendType, model);

        // Ops�� ����Ͽ� �ټ��� ���� ������ ������ �� �ֽ��ϴ�.
        ops = WorkerFactory.CreateOps(backendType, null);

        // �̹������� ������ ����� ī�޶� �����ɴϴ�.
        lookCamera = Camera.main;
    }

    // �̹����� �Ű�� �𵨷� ������ �� ������ Ȯ���� ��ȯ�մϴ�.
    public (float, int) GetMostLikelyDigitProbability(Texture2D drawableTexture)
    {
        inputTensor?.Dispose();

        // �ؽ�ó�� �ټ��� ��ȯ�մϴ�. �ʺ�=W, ����=W, ä��=1�Դϴ�.
        inputTensor = TextureConverter.ToTensor(drawableTexture, imageWidth, imageWidth, 1);

        // �Ű���� �����մϴ�.
        engine.Execute(inputTensor);

        // GPU�� ����� ������ ä �Ű���� ����� ����ϴ�.
        TensorFloat result = engine.PeekOutput() as TensorFloat;

        // ����Ʈ�ƽ� �Լ��� ����Ͽ� ����� 0���� 1 ������ Ȯ���� ��ȯ�մϴ�.
        var probabilities = ops.Softmax(result);
        var indexOfMaxProba = ops.ArgMax(probabilities, -1, false);

        // GPU���� ����� CPU���� ���� �� �ֵ��� ����ϴ�.
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

    // ���콺 Ŭ���� �����ϰ� ������ �г� Ŭ������ �����մϴ�.
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

    // ���콺�� ���� �ִ��� �����ϰ� ������ �г� Ŭ������ �����մϴ�.
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

    // ���� ����� ��� ���ҽ��� �����Ͽ� GPU �Ǵ� �޸𸮿� �������� �ʵ��� �մϴ�.
    private void OnDestroy()
    {
        inputTensor?.Dispose();
        engine?.Dispose();
        ops?.Dispose();
    }

}
