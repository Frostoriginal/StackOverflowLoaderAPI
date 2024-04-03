using System.Net;

namespace IntegrationTests
{
    public class IntegrationTests
    {
        [Fact(DisplayName = "Get call with no parameters - return ok, default parameters")]
        public async Task GetCallWithNoParameters()
        {
            // Arrange
            HttpClient client = new HttpClient();
            //string testRoute = "http://localhost:7142/api/TagData";
            string testRoute = "http://host.docker.internal:7142/api/TagData";
            

            // Act

            var response = await client.GetAsync(testRoute);
            
            // Assert            
            Assert.Equal( HttpStatusCode.OK, response.StatusCode);
        }
               
        
        [Fact(DisplayName = "Get call with correct parameters - page size")]
        public async Task GetCallWithParametersPageSize()
        {
            // Arrange
            HttpClient client = new HttpClient();            
            int pageSize = 1;
            string testRoute = $"http://host.docker.internal:7142/api/TagData?PageSize={pageSize}";
           
            // Act
            var response = await client.GetAsync(testRoute);            
            bool containsPageSize = response.Headers.ToString().Contains($"\"PageSize\":{pageSize}");
            

            // Assert
            Assert.True(containsPageSize);            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact(DisplayName = "Get call with correct parameters - page number")]
        public async Task GetCallWithParametersPageNumber()
        {
            // Arrange
            HttpClient client = new HttpClient();
            int pageNumber = 1;
            string testRoute = $"http://host.docker.internal:7142/api/TagData?PageNumber={pageNumber}";

            // Act
            var response = await client.GetAsync(testRoute);
            bool containsPageNumber = response.Headers.ToString().Contains($"\"CurrentPage\":{pageNumber}");

            // Assert
            Assert.True(containsPageNumber);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact(DisplayName = "Get call with correct parameters - page size and page number")]
        public async Task GetCallWithParametersPageSizeAndNumber()
        {
            // Arrange
            HttpClient client = new HttpClient();
            int pageNumber = 1;
            int pageSize = 1;
            string testRoute = $"http://host.docker.internal:7142/api/TagData?PageNumber={pageNumber}&PageSize={pageSize}";

            // Act
            var response = await client.GetAsync(testRoute);

            bool containsPageSize = response.Headers.ToString().Contains($"\"PageSize\":{pageSize}");
            bool containsPageNumber = response.Headers.ToString().Contains($"\"CurrentPage\":{pageNumber}");

            // Assert
            Assert.True(containsPageSize);
            Assert.True(containsPageNumber);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact(DisplayName = "Get call with incorrect parameters - page number out of range")]
        public async Task GetCallWithIncorrectParametersOutOfRange()
        {
            // Arrange
            HttpClient client = new HttpClient();
            int pageNumber = 1000000;            
            string testRoute = $"http://host.docker.internal:7142/api/TagData?PageNumber={pageNumber}";

            // Act
            var response = await client.GetAsync(testRoute);            

            // Assert            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }

        [Fact(DisplayName = "Get call with incorrect parameters - less than 1. Expected min Page Size = 1.")] //lower than 1 should default to 1
        public async Task GetCallWithIncorrectParametersPageSizeRevertLower()
        {
            // Arrange
            HttpClient client = new HttpClient();
           
            int pageSize = -1;
            int expectedPageSize = 1;
            string testRoute = $"http://host.docker.internal:7142/api/TagData?PageSize={pageSize}";

            // Act
            var response = await client.GetAsync(testRoute);
            bool containsPageSize = response.Headers.ToString().Contains($"\"PageSize\":{expectedPageSize}");
            

            // Assert
            Assert.True(containsPageSize);            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact(DisplayName = "Get call with incorrect parameters - over 50. Expected max Page Size = 50.")] //higher than 50 should default to 50
        public async Task GetCallWithIncorrectParametersPageSizeRevertHigher()
        {
            // Arrange
            HttpClient client = new HttpClient();

            int pageSize = 80;
            int expectedPageSize = 50;
            string testRoute = $"http://host.docker.internal:7142/api/TagData?PageSize={pageSize}";

            // Act
            var response = await client.GetAsync(testRoute);
            bool containsPageSize = response.Headers.ToString().Contains($"\"PageSize\":{expectedPageSize}");


            // Assert
            Assert.True(containsPageSize);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact(DisplayName = "Get call with incorrect parameters - not int")] 
        public async Task GetCallWithIncorrectParametersPageSizeNotInt()
        {
            // Arrange
            HttpClient client = new HttpClient();

            string pageSize = "test";
            string testRoute = $"http://host.docker.internal:7142/api/TagData?PageSize={pageSize}";

            // Act
            var response = await client.GetAsync(testRoute);
            

            // Assert            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }
    }
}