using System;
using System.Linq;
using EFLibrary;
using EFLibrary.Models;
using EFLibrary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LibLibrary.Tests
{
    public class TransferServiceTests
    {
        private LibraryContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new LibraryContext(options);
        }

        [Fact]
        public void TransferCopies_ShouldSucceed_WhenValidTransfer()
        {
            using var context = GetInMemoryDbContext();
            var logger = new Mock<ILogger<TransferService>>();
            var service = new TransferService(context, logger.Object);

            // Arrange
            var book = new Book { BookId = 1, Title = "Test Book", Edition = "1st Edition" }; // Add Edition
            var sourceLibrary = new Library
            {
                LibraryId = 1,
                LibraryName = "Source Library",
                Contact = "123-456-7890", // Required property
                Email = "source@library.com", // Required property
                LibraryAddress = "123 Source St" // Required property
            };
            var destinationLibrary = new Library
            {
                LibraryId = 2,
                LibraryName = "Destination Library",
                Contact = "987-654-3210", // Required property
                Email = "destination@library.com", // Required property
                LibraryAddress = "456 Destination St" // Required property
            };
            var sourceCopy = new Copie { BookId = 1, LibraryId = 1, NumberOfCopies = 5 };

            context.Books.Add(book);
            context.Libraries.AddRange(sourceLibrary, destinationLibrary);
            context.Copies.Add(sourceCopy);
            context.SaveChanges();

            // Act
            var result = service.TransferCopies(sourceCopy, 2, 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.NumberOfCopies); // Destination should now have 2 copies
            Assert.Equal(3, sourceCopy.NumberOfCopies); // Source should have 3 copies left
        }

        [Fact]
        public void TransferCopies_ShouldThrow_WhenSourceAndDestinationAreSame()
        {
            using var context = GetInMemoryDbContext();
            var logger = new Mock<ILogger<TransferService>>();
            var service = new TransferService(context, logger.Object);

            // Arrange
            var sourceCopy = new Copie { BookId = 1, LibraryId = 1, NumberOfCopies = 5 };
            context.Copies.Add(sourceCopy);
            context.SaveChanges();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.TransferCopies(sourceCopy, 1, 2));
            Assert.Equal("Source and destination libraries must be different.", ex.Message);
        }

        [Fact]
        public void TransferCopies_ShouldThrow_WhenNotEnoughCopies()
        {
            using var context = GetInMemoryDbContext();
            var logger = new Mock<ILogger<TransferService>>();
            var service = new TransferService(context, logger.Object);

            // Arrange
            var book = new Book { BookId = 1, Title = "Test Book", Edition = "1st Edition" }; // Add Edition
            var sourceLibrary = new Library
            {
                LibraryId = 1,
                LibraryName = "Source Library",
                Contact = "123-456-7890", // Required property
                Email = "source@library.com", // Required property
                LibraryAddress = "123 Source St" // Required property
            };
            var destinationLibrary = new Library
            {
                LibraryId = 2,
                LibraryName = "Destination Library",
                Contact = "987-654-3210", // Required property
                Email = "destination@library.com", // Required property
                LibraryAddress = "456 Destination St" // Required property
            };
            var sourceCopy = new Copie { BookId = 1, LibraryId = 1, NumberOfCopies = 1 }; // Only 1 copy

            context.Books.Add(book);
            context.Libraries.AddRange(sourceLibrary, destinationLibrary);
            context.Copies.Add(sourceCopy);
            context.SaveChanges();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.TransferCopies(sourceCopy, 2, 2)); // Try to transfer 2 copies
            Assert.Contains("At least one copy must remain", ex.Message);
        }

        [Fact]
        public void TransferCopies_ShouldThrow_WhenDestinationLibraryDoesNotExist()
        {
            using var context = GetInMemoryDbContext();
            var logger = new Mock<ILogger<TransferService>>();
            var service = new TransferService(context, logger.Object);

            // Arrange
            var sourceCopy = new Copie { BookId = 1, LibraryId = 1, NumberOfCopies = 5 };
            context.Copies.Add(sourceCopy);
            context.SaveChanges();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.TransferCopies(sourceCopy, 3, 2)); // Destination library ID 3 does not exist
            Assert.Contains("does not exist", ex.Message);
        }
    }
}
