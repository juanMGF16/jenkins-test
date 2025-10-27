using Entity.Models.Base;
using Entity.Models.System;

namespace Entity.Models.ParametersModule
{
    public class CategoryItem : GenericEntity
    {
        // Propiedades de Navegacion Inversa
        public List<Item> Items { get; set; } = [];
    }
}
