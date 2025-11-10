using UnityEngine;

public class TestShapeClick : MonoBehaviour
{
    public TestManager testManager;

    private void OnMouseDown()
    {
        testManager.ShapeClicked(this.gameObject);
    }
}
