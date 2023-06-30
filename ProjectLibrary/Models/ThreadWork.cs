using System.Threading;

namespace ProjectLibrary.Models
{
    public class ThreadWork
    {

        private WorkModel workModel;

        private WorkContent workContent;
        
        private WorkModel.DelVoidWork delVoidWorkUpdate;

        private WorkModel.DelVoidError delVoidError;

        public ThreadWork(WorkModel workModel,WorkContent workContent, WorkModel.DelVoidWork delVoidWorkUpdate, WorkModel.DelVoidError delVoidError)
        {
            this.workModel = workModel;
            this.workContent = workContent;
            this.delVoidWorkUpdate = delVoidWorkUpdate;
            this.delVoidError = delVoidError;
        }

        public WorkModel.DelVoidWork DelVoidWorkUpdate { get => delVoidWorkUpdate; set => delVoidWorkUpdate = value; }
        public WorkModel.DelVoidError DelVoidError { get => delVoidError; set => delVoidError = value; }

        public void Run()
        {
            try
            {
                this.workModel.LaunchWork(this.workContent, this.delVoidWorkUpdate,this.delVoidError);
            }
            catch (ThreadInterruptedException exception)
            {
            }
        }
    }
}
