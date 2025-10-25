using Domain;
using HumanResources.Domain;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Training.Domain;

namespace Training.AppLogic
{
    internal class CoachService : ICoachService
    {
        private readonly ITrainingRepository _trainingRepository;
        private readonly IRoomRepository _roomRepository;

        public CoachService(ITrainingRepository trainingRepository, IRoomRepository roomRepository)
        {
            _trainingRepository = trainingRepository;
            _roomRepository = roomRepository;
        }

        public async Task<Domain.Training> CreateTraining(string name, string description, int maximumCapacity, Code roomCode, string coachId, TimeWindow timeWindow)
        {
            int sequence = await _trainingRepository.GetNumberOfTrainingsByName(name) + 1;
            Room? room = await _roomRepository.GetByIdAsync(roomCode);

            Contracts.Require(room is not null, "The room with the given code does not exist.");


            var existingTrainings = await _trainingRepository.GetTrainingsByRoomCode(roomCode);
            Contracts.Require(!existingTrainings.Any(t => t.TimeWindow.Overlaps(timeWindow)), "The room is not available for the given time window.");


            Domain.Training training = Domain.Training.CreateNew(name, description, maximumCapacity, roomCode, coachId, timeWindow, sequence);
            await _trainingRepository.AddAsync(training);

            return training;
        }
    }
}