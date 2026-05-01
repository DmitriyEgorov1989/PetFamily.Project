using System.Collections;

namespace PetFamily.SharedKernel.Errors;

public sealed partial class Error
{
    public class ErrorList : IEnumerable<Error>
    {
        private readonly List<Error> _list;

        public ErrorList(List<Error> list)
        {
            _list = list;
        }

        public IEnumerator<Error> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Перегрузка оператора приведения
        //Если перегружаем коллекцию то возвращаем список ошибок
        public static implicit operator ErrorList(List<Error> errors)
        {
            return new ErrorList(errors);
        }

        //Если перегружаем одну ошибку то возвращаем список с одной ошибкой
        public static implicit operator ErrorList(Error error)
        {
            return new ErrorList(new List<Error>([error]));
        }
    }
}