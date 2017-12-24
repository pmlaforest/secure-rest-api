using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TodoApi.Controllers
{
    [Authorize]
    [Route("api/todos")]
    public class todosController : Controller
    {
        public todosController(UserTodoContext context)
        {
        }

       [HttpGet]
        public IEnumerable<Todo> GetAll()
        {
            long conUserId = DataAccess.GetConnectedUser();

            List<Todo> listTodo = null;

            if (conUserId == 0) {
                return listTodo;
            }

            listTodo = DataAccess.GetTodosList(conUserId);

            return listTodo;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Todo todo)
        {
            long conUserId = DataAccess.GetConnectedUser();

            if(conUserId == -1)
            {
                return new NotFoundResult();
            }

            DataAccess.AddUsersTodo(todo);

            return CreatedAtRoute("GetTodo", new { id = todo.Id }, todo) ;
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Todo todo)
        {
            if(todo == null || todo.Id != id)
            {
                return new BadRequestResult();
            }

            if (DataAccess.UpdateTodo(todo))
                return new OkResult();
            else
                return new NotFoundResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = DataAccess.GetTodo(id);
            if (todo == null)
                return new NotFoundResult();

            if (DataAccess.DeleteTodo(id))
                return new OkResult();
            else
                return new BadRequestResult();
        }
    }
}