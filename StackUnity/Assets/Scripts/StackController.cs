using UnityEngine;

public class StackController : MonoBehaviour
{
    #region Variables
    private GameObject[] theStack;                                        //Contendra los 12 cubos(object)
    private int cubeIndex;                                                //Cantidad de cubos a mover(x,y)
    private int cubeYMovement = 0;                                        //Representa la coordenada Y a moverse
    private const float CUBEBOUNDS = 4.0f;                                //Limite X,Z de los cubos al oscilar
    private float cubeSpeed = 2.5f;                                       //velocidad de los cubos
    private float transitionSpeed = 0f;                                   //velocidad de transicion entre cubos
    private bool oscilatingX = true;                                      //Controla oscilacion en eje X,Z
    private float newSecodPosition = 0f;                                  //Reposiciona el cubo en eje secundario al que oscila

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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))                                          //Si damos click
        {
            if (PlaceCubes())                                                     // si aun no pierde
            {
                SpawnCubes();                                                     //Coloca el nuevo cubo
                cubeYMovement++;                                                    //Aumenta coordenada Y de cubos
            }
            else GameOver();                                                     //Si perdimos activa el gameover

        }                                                                    
            MoveCubes();
    }

    private bool PlaceCubes()
    {
        Transform cubeT = theStack[cubeIndex].transform;                        //Ref a las coordenadas de los cubos
                                                                                //Actualiza la coordenada secundaria
        newSecodPosition = (oscilatingX) ? cubeT.localPosition.x : cubeT.localPosition.z;
        return true;
    }

    private void SpawnCubes()
    {
        cubeIndex--;                                                                //Cambia al siguiente cubo

        if (cubeIndex < 0)                                                          //Hace q vuelvas a mover el ultimo cubo
            cubeIndex = theStack.Length - 1;

        if (cubeIndex % 2 == 0)                                                     //Alterna la oscilacion basado en indice pares
            oscilatingX = false;
        else oscilatingX = true;
                                                                                    //Cambia la coordenada Y de los cubos
        theStack[cubeIndex].transform.localPosition = new Vector3(0, cubeYMovement, 0);
    }

    private void MoveCubes()
    {
        transitionSpeed += cubeSpeed * Time.deltaTime;                      //Velocidad de transicion
                                                                            //Hace Oscilar al cubo con una vel
        if (oscilatingX)                                                    //Oscilar ejeX
        {
            theStack[cubeIndex].transform.localPosition = new Vector3(Mathf.Sin(transitionSpeed) * CUBEBOUNDS, cubeYMovement, newSecodPosition);
        }
        else                                                               //Oscilar en eje Z
            theStack[cubeIndex].transform.localPosition = new Vector3(newSecodPosition, cubeYMovement, Mathf.Sin(transitionSpeed) * CUBEBOUNDS);

    }

    private void GameOver()
    {


    }
    #endregion
}
