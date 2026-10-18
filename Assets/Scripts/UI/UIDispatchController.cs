using UnityEngine;

public class UIDispatchController : MonoBehaviour
{  
    [SerializeField]
    private GameObject cd_1;
    [SerializeField]
    private GameObject cd_2;

    [SerializeField] private float spinSpeed_cd_1 = 30;
    [SerializeField] private float spinSpeed_cd_2 = 50f;

    
    // Update is called once per frame
    void Update()
    {
        if (DispatchController.Instance.InsertedKeycard != null)
        {
            cd_1.transform.Rotate(0, 0, -spinSpeed_cd_1 * Time.deltaTime);
            cd_2.transform.Rotate(0, 0,-spinSpeed_cd_2 * Time.deltaTime);
        }
        
    }
}
