using UnityEngine;
using UnityEngine.UI;

public class StackController : MonoBehaviour
{
    #region VariablesPrivadas
    private GameObject[] theStack;                                        //Contendra los 12 cubos(object)
    private int cubeIndex;                                                //Cantidad de cubos a mover(x,y)
    private int cubeYMovement = 0;                                        //Representa la coordenada Y a moverse
    private const float CUBEBOUNDS = 4.0f;                                //Limite X,Z de los cubos al oscilar
    private float cubeSpeed = 2.5f;                                       //Velocidad de los cubos
    private float transitionSpeed = 0f;                                   //Velocidad de transicion entre cubos
    private bool oscilatingX = true;                                      //Controla oscilacion en eje X,Z
    private float newSecodPosition = 0f;                                  //Reposiciona el cubo en eje secundario al que oscila
    private Vector3 theStackNewPos;                                       //Almacenara nueva pos del stack al dar click
    private const float STACKDOWNSPEED = 10.0f;                            //Velocidad q tardara en descender el stack
    private Vector3 lastCubePos;                                          //Guarda la previa pos del cubo
    private const float ERRORMARGIN = 0.25f;                               //margen de error al colocar el cubo en stack
    private int cubeCombos = 0;                                           //Combos Puntaje
    private Vector3 stackBounds = new Vector3(CUBEBOUNDS, 0, CUBEBOUNDS); //Mantener un registro de los lim X,Z del cubo al dar click
    private bool dropCube = false;                                        //bool q controla caida del cubo al fallar
    private const int CUBECOMBOSTART = 4;                                 //Inicio del combo
    private const float CUBECOMBOBONUS = 0.25f;                           //Incremento al tamano x mantener un combo de 5
    public Text scoreText;                                               //texto de puntaje nen pantalla
    #endregion

    #region Variables Publicas
    public Color32[] colors = new Color32[5];               //Colores de los bloques
    public Material rubbleMat;                              //Material para los escombros
    #endregion
    
    #region Metodos

    private void Start()
    {
        theStack = new GameObject[transform.childCount];                    //Definir tamano del array

        for (int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;                 //manda las coordenadas al array
            MeshColor(theStack[i].GetComponent<MeshFilter>().mesh);         //Asignar un color al iniciar
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
                cubeYMovement++;                                                  //Aumenta coordenada Y de cubos
                scoreText.text = cubeYMovement.ToString();                          
            }
            else GameOver();                                                     //Si perdimos activa el gameover

        //Mueve theStack completo hacia abajo cuando damos click
        transform.position = Vector3.Lerp(transform.position, theStackNewPos, STACKDOWNSPEED * Time.deltaTime);
        }                                                                    
            MoveCubes();

    }


    //----------------COLOCAR CUBOS DEL STACK----------------------//
    private bool PlaceCubes()
    {
        Transform cubeT = theStack[cubeIndex].transform;                   //Ref a las coordenadas de los cubos

        if (oscilatingX) //Reduccion de tamano de cubos al oscilar en X
        {
            float deltaX = lastCubePos.x - cubeT.position.x;                 //x1: pos. previa x2: pos. actual
            Debug.Log("diferencial x: " + Mathf.Abs(deltaX));

            if (Mathf.Abs(deltaX) > ERRORMARGIN)//Si es mayor Cortamos el cubo
            {
                cubeCombos = 0;                 //elimina el combo al fallar
                //reducir el limite del cubo en eje x
                stackBounds.x -= Mathf.Abs(deltaX);
                Debug.Log(" Valor lim de X:" + stackBounds.x);

                if (stackBounds.x <= 0) //Si limite menor igual a 0
                {
                    Debug.Log("GAME OVER x");
                    return false;
                }

                float middle = (lastCubePos.x + cubeT.localPosition.x) / 2;//Guarda el pto. medio de los 2 ultimos cubos

                //Asignar nueva Escala al cubo que se va a cortar
                cubeT.localScale = new Vector3(stackBounds.x, 1, stackBounds.z);
                Debug.Log("Escala cubos " + cubeT.localScale);

                //Crear efecto de escombro ejeX
                CreateRubble
                    ( 
                        new Vector3//Coordenadas del escombro
                        (
                            (cubeT.position.x > 0) ? cubeT.position.x + (cubeT.localScale.x/2) 
                                                   : cubeT.position.x - (cubeT.localScale.x/2),
                            cubeT.position.y,
                            cubeT.position.z
                        ),
                        new Vector3//escala del escombro
                        ( 
                            Mathf.Abs(deltaX),
                            1,
                            cubeT.localScale.z 
                        )
                    );

                //Reposicionar cubo cortado a partir de donde se hizo click
                cubeT.localPosition = new Vector3(middle - (lastCubePos.x / 2), cubeYMovement, lastCubePos.z);
            }
            else//Colocar cubo sin error
            {
                if (cubeCombos > CUBECOMBOSTART)
                {
                    //aumenta el limite del eje al mantenerse en combo
                    stackBounds.x += CUBECOMBOBONUS;

                    //Evita que se pase del tamano max
                    if (stackBounds.x > CUBEBOUNDS)
                        stackBounds.x = CUBEBOUNDS;

                    //Recalcula escala y coordenadas con el bonus
                    float middle = (cubeT.localPosition.x + lastCubePos.x) / 2;
                    cubeT.localScale = new Vector3(stackBounds.x, 1, stackBounds.z);
                    cubeT.localPosition = new Vector3(middle - (lastCubePos.x / 2), cubeYMovement, lastCubePos.z);

                }
                cubeCombos++;
                //Ignoramos el error de margen al colocar cubos
                cubeT.localPosition = new Vector3(lastCubePos.x, cubeYMovement, lastCubePos.z);
            }

        }
        else //Control al oscilar en Z
        {
            float deltaZ = lastCubePos.z - cubeT.position.z;        //dif en eje z entre los cubos

            if (deltaZ > ERRORMARGIN)   //Check entre las distancias de los cubos
            {
                cubeCombos = 0; //elimina el combo del player
                stackBounds.z -= Mathf.Abs(deltaZ);//reducir el limite del ejeZ del cubo al colocarse

                if (stackBounds.z <= 0)//Si el limite es 0  o menor
                {
                    Debug.Log("GAME OVER z");
                    return false;
                }

                //pto. medio entre los 2 cubos de arriba
                float middle = (cubeT.localPosition.z + lastCubePos.z) / 2;
                
                //Asignar nueva escala para simular corte 
                cubeT.localScale = new Vector3(stackBounds.x, 1, stackBounds.z);

                //Crear efecto de escombro ejeZ
                CreateRubble
                    (
                        new Vector3
                        (
                            cubeT.position.x,
                            cubeT.position.y,
                            (cubeT.position.z > 0) ? cubeT.position.z + (cubeT.localScale.z / 2)
                                                   : cubeT.position.z - (cubeT.localScale.z / 2)
                         ),
                         new Vector3
                         (
                             cubeT.localScale.x,
                             1,
                             Mathf.Abs(deltaZ)
                         )
                    );

                //Reubicar el cubo cortado ejez
                cubeT.localPosition = new Vector3(lastCubePos.x, cubeYMovement, middle - (lastCubePos.x / 2));

            }
            else//Colocar cubo sin error
            {
                if (cubeCombos > CUBECOMBOSTART)
                {
                    //aumenta el limite del eje al mantenerse en combo
                    stackBounds.z += CUBECOMBOBONUS;
                    
                    if (stackBounds.z > CUBEBOUNDS)
                        stackBounds.z = CUBEBOUNDS;

                    //Recalcula escala y coordenadas con el bonus
                    float middle = (cubeT.localPosition.z + lastCubePos.z) / 2;
                    cubeT.localScale = new Vector3(stackBounds.x, 1, stackBounds.z);
                    cubeT.localPosition = new Vector3(lastCubePos.x, cubeYMovement, middle - (lastCubePos.x / 2));
                }
                cubeCombos++;
                //Ignoramos el error de margen al colocar cubos
                cubeT.localPosition = new Vector3(lastCubePos.x, cubeYMovement, lastCubePos.z);
            }

        }

        //Actualiza la coordenada del eje secundaria al q se mueve 
        newSecodPosition = (oscilatingX) ? cubeT.localPosition.x : cubeT.localPosition.z;
        return true;
    }

    //----------------SELECCION DE CUBOS Y CAMBIO DE EJE----------//
    private void SpawnCubes()
    {
        //Guardamos la coordenada del cubo antes de pasar al siguiente
        lastCubePos = theStack[cubeIndex].transform.position;

        cubeIndex--;                                                                //Cambia al siguiente cubo

        if (cubeIndex < 0)                                             //Hace q vuelvas a mover el ultimo cubo
            cubeIndex = theStack.Length - 1;

        //Alterna la oscilacion basado en indice pares
        if (cubeIndex % 2 == 0)                                                    
            oscilatingX = false;
        else oscilatingX = true;

        //Guarda la nueva coordenada del stack
        theStackNewPos = (Vector3.down) * cubeYMovement;                            

        //Asignar Altura (y) a la cual se colocara el Nuevo cubo
        theStack[cubeIndex].transform.localPosition = new Vector3(0, cubeYMovement, 0);

        //Preserva la escala del cubo anterior 
        theStack[cubeIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.z);

        //Agregar colores al stack
        MeshColor(theStack[cubeIndex].GetComponent<MeshFilter>().mesh);
    }

    //-----------------------CONTROL DE OSCILACIONES X,Z----------------//
    private void MoveCubes()
    {
        if (dropCube) return;

        transitionSpeed += cubeSpeed * Time.deltaTime;                      //Velocidad de transicion
        //Hace Oscilar al cubo con una vel y actualiza coordenada en eje secundario
        if (oscilatingX)                                                    //Oscilar ejeX
        {
            theStack[cubeIndex].transform.localPosition = new Vector3(Mathf.Sin(transitionSpeed) * CUBEBOUNDS, cubeYMovement, newSecodPosition);
        }
        else                                                               //Oscilar en eje Z
            theStack[cubeIndex].transform.localPosition = new Vector3(newSecodPosition, cubeYMovement, Mathf.Sin(transitionSpeed) * CUBEBOUNDS);
  
    }

    //--------------------CREACION DE ESCOMBROS----------------------//
    private void CreateRubble(Vector3 pos, Vector3 scale )
    {
        //Crea un cubo externo al stack
        GameObject rubbleCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rubbleCube.transform.position = pos;
        rubbleCube.transform.localScale = scale;
        rubbleCube.AddComponent<Rigidbody>();//gravedad

        //Recuperar y asignar el material de escombros
        rubbleCube.GetComponent<MeshRenderer>().material = rubbleMat;
        //Darle el mismo color a los escombros
        MeshColor(rubbleCube.GetComponent<MeshFilter>().mesh);
    }

    //---------------TRANSICION DE COLORES--------------------//
    private Color32 ColorPicker(Color32 a,Color32 b, Color32 c, Color32 d, Color32 e, float t)
    {
        if (t < 0.30f) return Color32.Lerp(a, b, t / 0.30f);
        else if (t < 0.50f) return Color32.Lerp(b, c, (t - 0.30f) / 0.30f);
        else if (t < 0.60f) return Color32.Lerp(c, d, t / 0.60f);
        else return Color32.Lerp(d, e, (t - 0.60f) / 0.60f);
    }

    //------------------MODIFICACION DEL VERTEX SHADER------------//
    private void MeshColor(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;         //guarda las coordenadasMesh de los vertices de los cubos
        Color32[] cubeColors = new Color32[vertices.Length];//almacena colores q iran a los vertices

        //Variacion del time de los colores usando Funcion seno
        float time = Mathf.Sin(cubeYMovement * 0.45f);
        //Asigna un color a cada espacio del array
        for (int i = 0; i < vertices.Length; i++)
            cubeColors[i] = ColorPicker(colors[0], colors[1], colors[2], colors[3], colors[4], time);

        //pasar colores del array al mesh
        mesh.colors32 = cubeColors;
    }
        
    //----------------------FIN DEL JUEGO----------------------//
    private void GameOver()
    {
        dropCube = true;

        //Hace que el cubo caiga por gravedad
        theStack[cubeIndex].AddComponent<Rigidbody>();
    }
    #endregion
}
