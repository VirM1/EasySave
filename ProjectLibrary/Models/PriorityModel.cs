using System;
using System.Collections.Generic;
using System.Text;
using ProjectLibrary.PriorityLibrary;

namespace ProjectLibrary.Models
{
    class PriorityModel
    {
        private PriorityManager PriorityManager;

        public PriorityModel(PriorityManager PriorityManager)
        {
            this.PriorityManager = PriorityManager;
        }

        public void AddPriority(Priority app)
        {
            this.PriorityManager.AddPriority(app);
        }

        public void RemovePriority(Priority app)
        {
            this.PriorityManager.RemovePriority(app);
        }

        public void EditPriority(Priority app, string name)
        {
            this.PriorityManager.EditPriority(app, name);
        }
    }
}