/******************************************************************************
 * Struct: ErppuTribute.KeyValuePair
 * Description: "Overrides" the default KeyValuePair with a serializeable one
 * Author(s): Jonah Ahvonen, Matti Jokitulppo
 * Date: April 7, 2014
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ErppuTribute
{
    [Serializable]
    public struct KeyValuePair<K, V>
    {
        public K Key
        { get; set; }

        public V Value
        { get; set; }
    }
}
