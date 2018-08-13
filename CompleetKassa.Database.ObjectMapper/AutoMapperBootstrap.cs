using AutoMapper;

namespace CompleetKassa.Database.ObjectMapper
{
	public class ObjectMapperProvider
	{
		private IMapper m_mapper;
		public IMapper Mapper {
			get {
				return m_mapper;
			}

			private set
			{
				m_mapper = value;
			}

		}
		public ObjectMapperProvider()
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfiles(GetType().Assembly);
			});

			m_mapper = new Mapper(config);
		}
	}
}
