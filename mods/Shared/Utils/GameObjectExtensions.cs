using UnityEngine;

namespace SharedCode.Utils
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Traverse scene hierarchy topwards (parents) until a GameObject with a specific component is found.
        /// </summary>
        /// <typeparam name="TComponent">Component to look for</typeparam>
        /// <param name="instance">GameObject whose parent hierarchy shall be checked</param>
        /// <returns></returns>
        public static GameObject FindParentWithComponent<TComponent>( this GameObject instance )
        {
            GameObject parent = instance.transform.parent.gameObject;
            int maxTries = 100;

            while( parent != null && maxTries > 0 )
            {
                if( parent.GetComponent<TComponent>() != null )
                    return parent;

                parent = instance.transform.parent.gameObject;
                maxTries--;
            }

            return null;
        }
    }
}
