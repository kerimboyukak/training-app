using Domain;
using HumanResources.Domain;
using IntegrationEvents.Employee;
using MassTransit;


namespace HumanResources.AppLogic
{
    internal class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeFactory _employeeFactory;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPublishEndpoint _eventBus;


        public EmployeeService(IEmployeeFactory employeeFactory, IEmployeeRepository employeeRepository, IPublishEndpoint eventBus)
        {
            _employeeFactory = employeeFactory;
            _employeeRepository = employeeRepository;
            _eventBus = eventBus;
        }

        public async Task DismissAsync(EmployeeNumber employeeNumber, bool withNotice)
        {
            IEmployee? employeeToDismiss = await _employeeRepository.GetByNumberAsync(employeeNumber);
            Contracts.Require(employeeToDismiss != null, $"Cannot find an employee with number '{employeeNumber}'");
            employeeToDismiss!.Dismiss(withNotice);
            await _employeeRepository.CommitTrackedChangesAsync();
        }

        public async Task<IEmployee> HireNewAsync(string lastName, string firstName, DateTime startDate, EmployeeType type)
        {
            int sequence = await _employeeRepository.GetNumberOfStartersOnAsync(startDate) + 1;
            IEmployee newEmployee = _employeeFactory.CreateNew(lastName, firstName, startDate, sequence, type);
            await _employeeRepository.AddAsync(newEmployee);

            // DevOps module should be notified when a new developer is hired
            var devopsEvent = new EmployeeHiredIntegrationEvent 
            {
                Number = newEmployee.Number,
                LastName = newEmployee.LastName,
                FirstName = newEmployee.FirstName,
            };
            if (newEmployee.Type == EmployeeType.Developer)
            {
                await _eventBus.Publish(devopsEvent);
            }

            // Training module should be notified when a new employee is hired
            var trainingEvent = new EmployeeHiredForTrainingIntegrationEvent
            {
                Number = newEmployee.Number,
                LastName = newEmployee.LastName,
                FirstName = newEmployee.FirstName,
                Company = "KWSoft",
            };
            await _eventBus.Publish(trainingEvent);

            return newEmployee;
        }
        public async Task AppointEmployeeAsCoach(EmployeeNumber employeeNumber)
        {
            IEmployee? employeeToAppoint = await _employeeRepository.GetByNumberAsync(employeeNumber);
            Contracts.Require(employeeToAppoint != null, $"Cannot find an employee with number '{employeeNumber}'");
            employeeToAppoint!.AppointAsCoach();

            // Training module should be notified when an employee is appointed as a coach
            var @event = new EmployeeAppointedAsCoachIntegrationEvent
            {
                Number = employeeToAppoint.Number,
                LastName = employeeToAppoint.LastName,
                FirstName = employeeToAppoint.FirstName,
            };

            await _eventBus.Publish(@event);
            await _employeeRepository.CommitTrackedChangesAsync();
        }
    }
}
