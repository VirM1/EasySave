namespace EasySaveConsole.Controllers
{
    abstract class AbstractController
    {
        protected View view;

        protected AbstractController(View view)
        {
            this.view = view;
        }
    }
}
