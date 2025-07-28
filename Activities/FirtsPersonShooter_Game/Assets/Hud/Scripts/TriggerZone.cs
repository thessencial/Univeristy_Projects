//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public class TriggerZone : MonoBehaviour
//{

//    public TMP_Text m_MessageText;
//    private bool m_PlayerInZone = false;
//    private bool m_GameStarted = false;
//    // Start is called before the first frame update
//    void Start()
//    {
//        m_MessageText.text = "";
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player") && !m_GameStarted)
//        {
//            m_MessageText.text = "Pulsa E para empezar";
//            m_PlayerInZone = true;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            m_MessageText.text = "";
//            m_PlayerInZone = false;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (m_PlayerInZone && Input.GetKeyDown(KeyCode.E) && !m_GameStarted)
//        {
//            m_MessageText.text = "";
//            m_GameStarted = true;
//            foreach (MovingTarget target in FindObjectsOfType<MovingTarget>())
//            {
//                target.StartMoving();
//            }
//        }
//    }
//    public void EndGame()
//    {
//        m_MessageText.text = "Pulsa G si quieres repetir o Esc para seguir";
//        m_GameStarted = false;
//    }
//}
