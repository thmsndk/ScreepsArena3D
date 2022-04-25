using Assets.Scripts.ScreepsArenaApi;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets
{
    public class MainMenu : MonoBehaviour
    {
        private ListView arenaList;
        private ListView replayList;

        private void OnEnable()
        {
            var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

            // TODO: register callback onselect on arena list


            arenaList = rootVisualElement.Q<ListView>("ArenaListView");
            //arenaList.Add(); // TODO: unsure how we add something to a list in this new UI and how to style said element?

            StartCoroutine(GetArenas());
        }

        private IEnumerator GetArenas()
        {
            var http = new Http();
            Debug.Log("GetArenas");
            yield return http.GetArenaList(response =>
            {
                if (response.ok == 1)
                {
                    Debug.Log("Arenas:");
                    foreach (var arena in response.arenas)
                    {
                        Debug.Log($"{arena.name} {arena.rank} {arena.rating} {arena._id}");
                        // Capture the Flag 196 965 606873c364da921cb49855f7
                        // TODO: handle advanced as well
                    }
                }

            });
        }
    }
}
