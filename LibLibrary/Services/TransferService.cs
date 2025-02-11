﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace EFLibrary.Services
{
    public static class DbContextExtensions
    {
        public static bool IsInMemory(this DatabaseFacade database)
        {
            return database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        }
    }
    public class TransferService
    {
        private readonly LibraryContext _context;
        private readonly ILogger<TransferService> _logger;

        public TransferService(LibraryContext context, ILogger<TransferService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Copie TransferCopies(Copie sourceCopie, int destinationLibraryId, int quantity)
        {
            var trackedSourceCopie = ValidateTransfer(sourceCopie, destinationLibraryId, quantity);

            // Skip transactions for in-memory database
            if (!_context.Database.IsInMemory())
            {
                using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        return PerformTransfer(trackedSourceCopie, destinationLibraryId, quantity);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error transferring copies.");
                        throw;
                    }
                }
            }
            else
            {
                return PerformTransfer(trackedSourceCopie, destinationLibraryId, quantity);
            }
        }

        private Copie PerformTransfer(Copie trackedSourceCopie, int destinationLibraryId, int quantity)
        {
            var destinationCopie = _context.Copies
                .SingleOrDefault(c => c.BookId == trackedSourceCopie.BookId && c.LibraryId == destinationLibraryId);

            if (destinationCopie == null)
            {
                destinationCopie = new Copie
                {
                    BookId = trackedSourceCopie.BookId,
                    LibraryId = destinationLibraryId,
                    NumberOfCopies = quantity,
                };
                _context.Copies.Add(destinationCopie);
            }
            else
            {
                destinationCopie.NumberOfCopies += quantity;
            }

            trackedSourceCopie.NumberOfCopies -= quantity;

            _context.SaveChanges();
            return destinationCopie;
        }

        private Copie ValidateTransfer(Copie sourceCopie, int destinationLibraryId, int quantity)
        {
            if (_context == null) throw new ArgumentNullException(nameof(_context));
            if (sourceCopie == null) throw new ArgumentNullException(nameof(sourceCopie));
            if (quantity <= 0) throw new ArgumentException($"Invalid quantity: {quantity}. Must be greater than zero.");
            if (sourceCopie.LibraryId == destinationLibraryId)
                throw new InvalidOperationException("Source and destination libraries must be different.");

            var destinationLibrary = _context.Libraries.FirstOrDefault(l => l.LibraryId == destinationLibraryId);
            if (destinationLibrary == null)
                throw new InvalidOperationException($"The destination library (ID: {destinationLibraryId}) does not exist.");

            var trackedSourceCopie = _context.Copies.Find(sourceCopie.BookId, sourceCopie.LibraryId);
            if (trackedSourceCopie == null)
                throw new InvalidOperationException("The source copy does not exist in the database.");

            if (trackedSourceCopie.NumberOfCopies - quantity < 1)
                throw new InvalidOperationException($"Invalid transfer: At least one copy must remain. Current: {trackedSourceCopie.NumberOfCopies}, transferring: {quantity}.");

            return trackedSourceCopie;
        }
    }

}

