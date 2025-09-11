// using UnityEngine;

// public class ServiceStatusChecker : MonoBehaviour
// {
//     public MonoService service;

//     public void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.D))
//         {
//             if (service == null) return;
//             Debug.Log("=== Service Status ===");
//             Debug.Log($"{service.GetType()} AllServicesReady: {service.AllServicesReady}");
//             Debug.Log($"Required services remaining: {service.requiredServices.Count}");
//             foreach (var service in service.Services)
//             {
//                 Debug.Log($"Has service: {service.Key.Name}");
//             }
//         }
//     }
// }
