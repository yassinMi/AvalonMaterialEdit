using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonMaterialEdit
{

    public enum TextEditorMode
    {
        /// <summary>
        /// no special configuration
        /// </summary>
        Custom,
        /// <summary>
        /// configured to highlight and assit with where expression, e,g  <see cref="Price &lt; 8"/> 
        /// </summary>
        SqlFilterExpession,
        /// <summary>
        /// configured to highlight and assit with sql code 
        /// </summary>
        SqlCode
    }
}
