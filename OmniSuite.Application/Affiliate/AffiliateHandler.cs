using OmniSuite.Application.Affiliate.Commands;
using OmniSuite.Application.Affiliate.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSuite.Application.Affiliate
{
    public class AffiliateHandler :
        IRequestHandler<CreateAffiliateCommand, Response<CreateAffiliateResponse>>
    {

        private readonly ApplicationDbContext _context;

        public AffiliateHandler(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
        }

        public async Task<Response<CreateAffiliateResponse>> Handle(CreateAffiliateCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
