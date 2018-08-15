using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.ViewModels
{
    public class SearchOperationModel
    {
		public NewOperationFormModel searchFields { get; set; }


		public IEnumerable<SingleOperationViewModel> ViewOperations { get; set; }


	}
}
