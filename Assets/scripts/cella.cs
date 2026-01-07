using UnityEngine;

public class cella : MonoBehaviour
{

    public GameObject muroSx;
    public GameObject muroDx;
    public GameObject muroSu;
    public GameObject muroGiu;

   
    public bool visitata { get; private set; } = false;

    public void cellaVisitata()
    {
        visitata = true;
    }

}