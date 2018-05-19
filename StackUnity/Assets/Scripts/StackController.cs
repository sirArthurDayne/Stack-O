using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackController : MonoBehaviour
{
    #region Variables
    private GameObject[] theStack;                                        //Contendra los 12 cubos(object)
    private int cubeIndex;                                                //Cantidad de cubos a mover(x,y)
    private int score = 0;                                                //Puntos en el juego
    #endregion

    #region Metodos

    private void Start()
    {
        theStack = new GameObject[transform.childCount];                    //Definir tamano del array

        for (int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;                 //manda las coordenadas al array
        }

        cubeIndex = theStack.Length - 1;                                    //guarda cantidad cubos en lista
        Debug.Log(cubeIndex);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))                                          //Si damos click
        {
            PlaceTiles();                                                         //Coloca el nuevo cubo
            score++;
            cubeIndex--;
            if (cubeIndex < 0)
                cubeIndex = theStack.Length - 1;
        }                                                                    
    }

    private void PlaceTiles()
    {
        theStack[cubeIndex].transform.localPosition = new Vector3(0,score,0);
    }
    #endregion
}
