using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class generatoreLab : MonoBehaviour
{
    int larghezza = 5;
    int altezza = 5;
    public GameObject prefabCella;
    public GameObject prefabGiocatore;
    public GameObject prefabUscita;
    public GameObject testoVittoria;

    public Text testoCronometro;
    public Text testoLivello;
    bool cronFermo = false;

    List<GameObject> celle = new List<GameObject>();  
    GameObject giocatore;
    GameObject uscita;
    int livello = 1;
    float tempo = 0;

    Camera cam;

    private void Start()
    {
        cam = Camera.main;
        CreaLabirinto();
        AggiornaCamera();
        AggiornaLivello();
    }

    private void Update()
    {
        if (!cronFermo)
        {
            tempo += Time.deltaTime;
            AggiornaCronometro();
        }
        
    }

    public void AggiornaCronometro()
    {
        if (testoCronometro == null)
        {
            return;
        }

        int minuti = (int)(tempo / 60);
        int secondi = (int)(tempo % 60);


        testoCronometro.text = $"TEMPO: {minuti:00}:{secondi:00}";
    }

    public void AggiornaLivello()
    {
        if (testoLivello == null)
        {
            return;
        }
        testoLivello.text = $"LIVELLO {livello}";
    }

    public void ProssimoLivello()
    {
        livello++;
        if (livello > 5)
        {
            MostraVittoria();
            return; 
        }


      
        if (livello == 2) {
           
            larghezza = 8; 
            altezza = 8; 
        }
        else if (livello == 3) { 
            larghezza = 10; 
            altezza = 10; 
        }
        else if (livello == 4) { 
            larghezza = 12; 
            altezza = 12; 
        }
        else if (livello == 5) { 
            larghezza = 15;
            altezza = 15;
        }

        CreaLabirinto();
        AggiornaCamera();
        AggiornaLivello();
    }

    private void MostraVittoria()
    {
        cronFermo = true;   
        foreach (GameObject c in celle)
        {
            if (c != null)
            {
                Destroy(c);
            }

        }
        celle.Clear();

        if (giocatore != null)
        {
            Destroy(giocatore);
        }

        if (uscita != null)
        {
            Destroy(uscita);
        }

       
        if (testoVittoria != null)
        {
            testoVittoria.SetActive(true);
            
        }

        

    }

    private void CreaLabirinto()
    {
       
        foreach (GameObject c in celle)
        {
            if (c != null)
            {
                Destroy(c);
            }
        }
        celle.Clear();  

       
        for (int riga = 0; riga < larghezza; riga++)
        {
            for (int colonna = 0; colonna < altezza; colonna++)
            {
                // moltiplico per 6 perchè le celle occupano 6
                float x = riga * 6f;
                float y = colonna * 6f;

                
                GameObject nuovaCella = Instantiate(prefabCella); // crea una copia del prefab cella
                nuovaCella.transform.position = new Vector3(x, y, 0);
               
                celle.Add(nuovaCella);
            }
        }

        
        cella[,] griglia = new cella[larghezza, altezza];

        
        int i = 0;
        for (int riga = 0; riga < larghezza; riga++)
        {
            for (int colonna = 0; colonna < altezza; colonna++)
            {
                griglia[riga, colonna] = celle[i].GetComponent<cella>(); // mi permette di accedera alle variabili della cella, posso vedere per esempio se è visitata
                i++;
            }
        }

        
        GeneraLab(griglia, 0, 0);

        
        Posizionamento();
    }

    private void AggiornaCamera()
    {
        if (cam == null)
        {
            return;
        }

        
        float centroX = (larghezza * 6f) / 2f;
        float centroY = (altezza * 6f) / 2f;

      
        cam.transform.position = new Vector3(centroX -2, centroY - 2, -10); // diminuisco di 2 il centro così che venga più centrato

        if (livello == 1)
        {
            cam.orthographicSize = 18f;  
        }
        else if (livello == 2)
        {
            cam.orthographicSize = 28f; 
        }
        else if (livello == 3)
        {
            cam.orthographicSize = 35f;  
        }
        else if (livello == 4)
        {
            cam.orthographicSize = 42f; 
        }
        else if (livello == 5)
        {
            cam.orthographicSize = 52f;  
        }
    }
    


    private void GeneraLab(cella[,] griglia, int riga, int colonna)
    {
        griglia[riga, colonna].cellaVisitata();

        List<Direzione> direzioni = new List<Direzione>();
        direzioni.Add(new Direzione(0, 1)); // su
        direzioni.Add(new Direzione(1, 0)); // dx
        direzioni.Add(new Direzione(0, -1)); // giu
        direzioni.Add(new Direzione(-1, 0)); // sx

        MescolaDirezioni(direzioni); // faccio in modo che la prima direzione che si prova non sia sempre la stessa

        foreach (Direzione dir in direzioni)
        {
            int nuovaRiga = riga + dir.x;
            int nuovaColonna = colonna + dir.y;

            if (PosizioneValida(nuovaRiga, nuovaColonna, griglia.GetLength(0), griglia.GetLength(1)) &&
                !griglia[nuovaRiga, nuovaColonna].visitata) // controllo se cella esiste ed è stata visitata
            {
                RimuoviMuro(griglia, riga, colonna, nuovaRiga, nuovaColonna);
                GeneraLab(griglia, nuovaRiga, nuovaColonna);
            }
        }
    }

    private bool PosizioneValida(int riga, int colonna, int maxRighe, int maxColonne)
    {
        return riga >= 0 && riga < maxRighe && colonna >= 0 && colonna < maxColonne;
    }

    private void RimuoviMuro(cella[,] griglia, int riga1, int colonna1, int riga2, int colonna2)
    {
        int diffRiga = riga2 - riga1;
        int diffColonna = colonna2 - colonna1;

        // va rimosso il muro della cella visitata e quello adiacente ad essa
        if (diffRiga == 1)  // Destra
        {
            griglia[riga1, colonna1].muroDx.SetActive(false);
            griglia[riga2, colonna2].muroSx.SetActive(false);
        }
        else if (diffRiga == -1)  // Sinistra
        {
            griglia[riga1, colonna1].muroSx.SetActive(false);
            griglia[riga2, colonna2].muroDx.SetActive(false);
        }
        else if (diffColonna == 1)  // Su
        {
            griglia[riga1, colonna1].muroSu.SetActive(false);
            griglia[riga2, colonna2].muroGiu.SetActive(false);
        }
        else if (diffColonna == -1)  // Giu
        {
            griglia[riga1, colonna1].muroGiu.SetActive(false);
            griglia[riga2, colonna2].muroSu.SetActive(false);
        }
    }

    private void MescolaDirezioni(List<Direzione> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            int r = Random.Range(i, lista.Count);

            Direzione d = lista[i];
            lista[i] = lista[r];
            lista[r] = d;
        }
    }

    private void Posizionamento()
    {
        // giocatore
        if (giocatore == null && prefabGiocatore != null)
        {
            giocatore = Instantiate(prefabGiocatore);
        }
        if (giocatore != null)
        {
            giocatore.transform.position = new Vector3(0, 0, -1);
        }

        // uscita, facccio *6 perchè ogni cella occupa 6
        float posX = (larghezza - 1) * 6f;
        float posY = (altezza - 1) * 6f;

        if (uscita == null && prefabUscita != null)
        {
            uscita = Instantiate(prefabUscita);
        }
        if (uscita != null)
        {
            uscita.transform.position = new Vector3(posX, posY, -1);
        }
    }

  



}

