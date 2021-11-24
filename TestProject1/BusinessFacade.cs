using ECC.Customer.BusinessFacade;
using ECC.Customer.DataAccessLayer;
using ECC.Customer.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject1
{
    [TestClass]
    public class BusinessFacade
    {

        private Mock<IGenericRepository<PersonDto>> _personRepoMock;
        private ICustomerBusinessFacade _customerBusinesFacade;

        [TestInitialize]
        public void TestInitialize()
        {
            _personRepoMock = new Mock<IGenericRepository<PersonDto>>();
            _customerBusinesFacade = new CustomerBusinessFacade(_personRepoMock.Object);
        }

        [TestMethod]
        public void Add_Person_WithMissingName_ReturnsMissingParamsCode()
        {
            //arrange
            var person = new PersonDto { Id = 0, FirstName = null, LastName = "Smith" };
            _personRepoMock.Setup(p => p.Add(person)).Returns(person);

            //act
            var result = _customerBusinesFacade.Add(person);

            //assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsTrue(result.Code.Equals(BusinessCodes.CrudResultCodes.CODE_MANDATORY_VALUES_MISSING));

        }

        [TestMethod]
        public void Add_Person_ReturnsSuccessfull()
        {
            //arrange
            var person = new PersonDto { Id = 1, FirstName = "Able", LastName = "Smith" };
            _personRepoMock.Setup(p => p.Add(person)).Returns(person);

            //act
            var result = _customerBusinesFacade.Add(person);

            //assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsTrue(result.EntityId == 1);

        }


        [TestMethod]
        public void Get_PersonNotFound_ReturnsSuccessfullAndNull()
        {
            //arrange            
            _personRepoMock.Setup(p => p.GetById(1)).Returns((PersonDto)null);

            //act
            var result = _customerBusinesFacade.Get(1);

            //assert
            Assert.IsTrue(result.Item1.IsSuccessful);
            Assert.IsNull(result.Item2);

        }


    }
}
