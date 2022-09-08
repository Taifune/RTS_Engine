using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FloatingTextUI : MonoBehaviour
{

    public static FloatingTextUI Instance { get; private set; }

    private class FloatingText
    {
        public Text UIText;
        public float maxTime;
        public float timer;
        public Vector3 objectPosition;
        public Color color;
        public int x;
        public int y;
        public bool isWorldObject = true;

        public void MoveText(Camera cam,bool isWorldObject)
        {
            //Дельта перемещения 
            float delta = 1.0f - (timer / maxTime);
            Vector3 pos = objectPosition + new Vector3(delta*x, delta*y, .0f);

            if(isWorldObject)
            {
                pos = cam.WorldToScreenPoint(pos);
                pos.z = .0f;
            }            

            UIText.transform.position = pos;

        }

        public FloatingText() { }
        public FloatingText(Text UIText, float maxTime, float timer, Vector3 objectPosition, Color color)
        {
            this.UIText = UIText;
            this.maxTime = maxTime;
            this.timer = timer;
            this.objectPosition = objectPosition;
            this.color = color;
            x = 1;
            y = 1;
            isWorldObject = true;
        }
        public FloatingText(Text UIText, float maxTime, float timer, Vector3 objectPosition, Color color,int x,int y,bool isWorldObject)
        {
            this.UIText = UIText;
            this.maxTime = maxTime;
            this.timer = timer;
            this.objectPosition = objectPosition;
            this.color = color;
            this.x = x;
            this.y = y;
            this.isWorldObject = isWorldObject;
        }
    }


    

    const int PoolSize = 64;

    public Text m_TextPrefab;

    Queue<Text> m_TextPool = new Queue<Text>();

    List<FloatingText> m_FloatingText = new List<FloatingText>();


    private void Awake()
    {
        Instance = this;

        
    }

    Camera m_cameraMain;
    Transform m_Transform;

    // Start is called before the first frame update
    void Start()
    {
        m_cameraMain = Camera.main;
        m_Transform = transform;

        for(int i = 0; i<PoolSize;i++)
        {

            Text temp = Instantiate(m_TextPrefab,m_Transform);
            temp.gameObject.SetActive(false);
            m_TextPool.Enqueue(temp);
        }

    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<m_FloatingText.Count;i++)
        {
            FloatingText ft = m_FloatingText[i];
            ft.timer -= Time.deltaTime;

            if(ft.timer<=.0f)
            {
                ft.UIText.gameObject.SetActive(false);
                m_TextPool.Enqueue(ft.UIText);
                m_FloatingText.RemoveAt(i);
                --i;

            }
            else
            {
                var color = ft.UIText.color;
                color.a = ft.timer / ft.maxTime;
                ft.UIText.color = color;

                ft.MoveText(m_cameraMain,ft.isWorldObject);
            }

        }
    }

    public void AddTextFromFloat(float amount, Vector3 objectPosition, Vector3 textStartPoint, Color color)
    {

        var a = (int)amount;
        var t = m_TextPool.Dequeue();
        t.text = a.ToString();
        t.gameObject.SetActive(true);

        FloatingText ft = new FloatingText() { maxTime = 1.0f };
        ft.timer = ft.maxTime;
        ft.UIText = t;
        ft.color = color;
        ft.UIText.color = ft.color;
        ft.objectPosition = objectPosition + textStartPoint;
        ft.x = 1;
        ft.y = 1;
        ft.isWorldObject = true;
        ft.MoveText(m_cameraMain,ft.isWorldObject);
        m_FloatingText.Add(ft);
    }
      

    public void AddTextFromFloat(float amount, Vector3 objectPosition, Vector3 textStartPoint, Color color,int x,int y,bool isWorldObject)
    {

        var a = (int)amount;
        var t = m_TextPool.Dequeue();
        t.text = a.ToString();
        t.gameObject.SetActive(true);

        FloatingText ft = new FloatingText() { maxTime = 1.0f };
        ft.timer = ft.maxTime;
        ft.UIText = t;
        ft.color = color;
        ft.UIText.color = ft.color;
        ft.objectPosition = objectPosition + textStartPoint;
        ft.x = x;
        ft.y = y;
        ft.isWorldObject = isWorldObject;
        ft.MoveText(m_cameraMain,ft.isWorldObject);
        m_FloatingText.Add(ft);
    }


    public void AddText(string text, Vector3 objectPosition, Vector3 textStartPoint, Color color)
    {


        var t = m_TextPool.Dequeue();
        t.text = text;
        t.gameObject.SetActive(true);

        FloatingText ft = new FloatingText() { maxTime = 1.0f };
        ft.timer = ft.maxTime;
        ft.UIText = t;
        ft.color = color;
        ft.UIText.color = ft.color;
        ft.objectPosition = objectPosition + textStartPoint;
        ft.x = 1;
        ft.y = 1;
        ft.isWorldObject = true;
        ft.MoveText(m_cameraMain,ft.isWorldObject);
        m_FloatingText.Add(ft);
    }



    public void AddText(string text, Vector3 objectPosition, Vector3 textStartPoint, Color color,int x,int y,bool isWorldObject)
    {

        
        var t = m_TextPool.Dequeue();
        t.text = text;
        t.gameObject.SetActive(true);

        FloatingText ft = new FloatingText() { maxTime = 1.0f };
        ft.timer = ft.maxTime;
        ft.UIText = t;
        ft.color = color;
        ft.UIText.color = ft.color;
        ft.objectPosition = objectPosition + textStartPoint;
        ft.x = x;
        ft.y = y;
        ft.isWorldObject = isWorldObject;
        ft.MoveText(m_cameraMain,ft.isWorldObject);
        m_FloatingText.Add(ft);
    }
}
