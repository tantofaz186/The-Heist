using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AddCam : NetworkBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] GameObject head;
    private PlayerActions _actions;
    public TMP_Text useText;
    
    
    private void Start()
    {
        if (IsOwner)
        {   
            var thisCam = Instantiate(cam, head.transform.position, cam.transform.rotation, head.transform);
             _actions = GetComponent<PlayerActions>();
             _actions._camera = thisCam.GetComponent<Camera>();
             GameObject canvas = thisCam.gameObject.transform.GetChild(0).gameObject;
             GameObject useTextobj = canvas.transform.GetChild(0).gameObject;
             useText= useTextobj.GetComponent<TMP_Text>();
             Image keySprite = useTextobj.transform.GetChild(0).gameObject.GetComponent<Image>();
             _actions.useText = useText;
             _actions.keySprite = keySprite;
             _actions.ready = true;
        }
    }
}
