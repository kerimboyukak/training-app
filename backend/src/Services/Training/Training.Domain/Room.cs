using Domain;

namespace Training.Domain
{
    public class Room : Entity
    {
        public Code RoomCode { get; private set; }
        public string Name { get; private set; }

        private Room(string name)
        {
            Contracts.Require(!string.IsNullOrEmpty(name), "The name of the room cannot be empty");

            RoomCode = new Code(name, 1);
            Name = name;
        }

        public static Room CreateNew(string name)
        {
            return new Room(name);
        }

        protected override IEnumerable<object> GetIdComponents()
        {
            yield return RoomCode;
        }
    }
}