using System;
using System.Collections.Generic;
using System.Text;
using ProjectLibrary.ForbiddenFilesLibrary;

namespace ProjectLibrary.Models
{
    class ForbiddenFilesModel
    {
        private ForbiddenFilesManager forbiddenFilesManager;

        public ForbiddenFilesModel(ForbiddenFilesManager forbiddenFilesManager)
        {
            this.forbiddenFilesManager = forbiddenFilesManager;
        }

        public void AddForbiddenFile(ForbiddenApp app)
        {
            this.forbiddenFilesManager.AddForbiddenFile(app);
        }

        public void RemoveForbiddenFile(ForbiddenApp app)
        {
            this.forbiddenFilesManager.RemoveForbiddenFile(app);
        }

        public void EditForbiddenFile(ForbiddenApp app, string name)
        {
            this.forbiddenFilesManager.EditForbiddenFile(app, name);
        }
    }
}
