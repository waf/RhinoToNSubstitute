using Xunit;

namespace RhinoToNSubstitute.Tests.MockRepository
{
    public class MockRepositoryTests
    {
        [Theory]
        [InlineData("MockRepository", "GenerateStubMock")]
        public void Converts(string testCategory, string testCase) =>
            SyntaxSnapshot.RunSnapshotTest(testCategory, testCase);
    }
}
