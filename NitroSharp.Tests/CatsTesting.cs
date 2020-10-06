using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using NitroSharp.Utils;

using NUnit.Framework;

namespace NitroSharp.Tests
{
    public class CatsTesting
    {
        HttpClient Http;

        [OneTimeSetUp]
        public void SetUp()
        {
            Http = new HttpClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Http.Dispose();
        }

        [Test]
        public async Task GetCatJSONTest()
        {
            var data = await Cats.GetCatByIdAsync("qsGk4el-D");

            Assert.NotNull(data);
        }

        [Test]
        public async Task CatArgumentsTest()
        {
            var res = await Cats.GetCatDataAsync();

            Assert.NotNull(res);

            res = await Cats.GetCatDataAsync("desc");

            Assert.NotNull(res);
            Assert.NotZero(res.Length, "Missing Cat item");

            res = await Cats.GetCatDataAsync("random", "gif");

            Assert.NotNull(res);
            Assert.NotZero(res.Length, "Missing Cat item");
            Assert.True(Path.GetExtension(res[0].Url).Contains("gif"), "File type incorrect");

            res = await Cats.GetCatDataAsync("gif", "jpg", "random");

            Assert.NotNull(res);
            Assert.NotZero(res.Length, "Missing Cat item");
            Assert.True(Path.GetExtension(res[0].Url).Contains("jpg")
                || Path.GetExtension(res[0].Url).Contains("gif"), "File type incorrect.");

            res = await Cats.GetCatDataAsync("foo", "bar");

            Assert.NotNull(res);
            Assert.NotZero(res.Length, "Missing Cat item");
        }
    }
}
