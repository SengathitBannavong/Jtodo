using Jtodo.Commands;

namespace Jtodo.Models
{
    class HomeModel
    {
        private List<Context> _contexts;
        public HomeModel() {
            _contexts = new List<Context>();
        }

        public List<Context> GetListContexts() => _contexts;
        public void AddContext(Context context) { 
            _contexts.Add(context);
        }

        public void RemoveContext(Context context) { 
            _contexts.Remove(context);
        }

        public void MockData() {
            _contexts.Add(new Context("Buy groceries", "Milk, Bread, Eggs", DateTime.Now.AddDays(2), 1));
            _contexts.Add(new Context("Finish project", "Complete the Jtodo project", DateTime.Now.AddDays(5), 2));
            _contexts.Add(new Context("Workout", "Go to the gym for a workout session", DateTime.Now.AddDays(1), 3));
        }
    }
}
