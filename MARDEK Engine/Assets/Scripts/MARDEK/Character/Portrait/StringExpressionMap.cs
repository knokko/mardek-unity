using UnityEngine;
using System.Collections.Generic;

namespace MARDEK.CharacterSystem
{
    public class StringExpressionMap : MonoBehaviour
    {
        [field: SerializeField] public List<PortraitExpression> Map;

        public PortraitExpression GetExp(string name)
        {
            return Map.Find(expr => expr.name == name);
        }
    }
}
