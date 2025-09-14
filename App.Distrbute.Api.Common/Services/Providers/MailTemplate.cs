namespace App.Distrbute.Api.Common.Services.Providers;

public class MailTemplate
{
    private static readonly string Otp = 
        """
        <html>
            <head>
              <meta charset="UTF-8" />
              <meta name="viewport" content="width=device-width, initial-scale=1.0" />
              <title>One-Time Login Code</title>
            </head>
            <body style="font-family: sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;">
              <div style="max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
                
                <p style="font-size: 16px; color: #333; margin-bottom: 20px;">Hi {{name}},</p>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                  Here’s your one-time login code:
                </p>
                
                <p style="font-size: 18px; text-align: center; margin: 25px 0;">
                  Prefix: <strong>{{otpPrefix}}</strong>&nbsp;&nbsp;&nbsp;
                  <span style="font-size: 20px; background-color: #f0f0f0; padding: 8px 12px; border-radius: 6px;">🔑 {{otpCode}}</span>
                </p>
                
                <div style="background-color: #e8f4fd; border: 1px solid #b6e0fe; padding: 15px; border-radius: 5px; margin-bottom: 25px;">
                  <p style="font-size: 15px; color: #084298; margin: 0;">
                    <strong>Note:</strong> This code is valid for the next <strong>{{ttl}} minutes</strong>.  
                    Please don’t share it with anyone — even if they claim to be from our team.
                  </p>
                </div>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                  If you didn’t request this, you can safely ignore this message.
                </p>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 0;">
                  Thanks,<br/>
                  The Distrbute Team
                </p>
              </div>
              
              <div style="text-align: center; margin-top: 30px; color: #666; font-size: 14px;">
                <p>© {{year}} Distrbute. All rights reserved.</p>
              </div>
            </body>
        </html>
        """;
    
    private static readonly string PostDisputed =
        """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Post Disputed</title>
        </head>
        <body style="font-family: sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;">
            <div style="max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
                
                <p style="font-size: 16px; color: #333; margin-bottom: 20px;">Hey {{name}},</p>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    Your <a href="https://www.distrbute.com/post/{{postId}}">post</a> for <strong>{{brandName}}</strong> has been <strong>disputed</strong> by the brand.  
                    This means that the payout for this post ({{amount}}) is currently on hold until the dispute is resolved.
                </p>

                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    Post details:
                </p>

                <ul style="font-size: 16px; color: #333; margin-bottom: 25px; line-height: 1.6;">
                    <li><strong>Brand:</strong> {{brandName}}</li>
                    <li><strong>Platform:</strong> {{platform}}</li>
                    <li><strong>Content Type:</strong> {{contentType}}</li>
                    <li><strong>Pending Payout:</strong> {{amount}}</li>
                </ul>

                <div style="background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin-bottom: 25px;">
                    <p style="font-size: 16px; color: #856404; margin: 0;">
                        ℹ️ Our team will review the dispute and work to resolve it fairly.  
                        You don’t need to take action right now, but we may contact you if more information is required.
                    </p>
                </div>

                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    You can track the status of this dispute and your other posts anytime from your dashboard:
                </p>

                <p style="margin-bottom: 25px;">
                    👉 <a href="https://www.distrbute.com/dashboard" 
                          style="color: #2f80ed; text-decoration: none; font-weight: bold; font-size: 16px;">
                        Go to Dashboard
                    </a>
                </p>

                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 0;">
                    Thanks for your patience,<br/>
                    The Distrbute Team
                </p>
            </div>
            
            <div style="text-align: center; margin-top: 30px; color: #666; font-size: 14px;">
                <p>© {{year}} Distrbute. All rights reserved.</p>
            </div>
        </body>
        </html>
        """;

    private static readonly string PostAutoApproved =
        """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Post Auto-Approved</title>
        </head>
        <body style="font-family: sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;">
            <div style="max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
                
                <p style="font-size: 16px; color: #333; margin-bottom: 20px;">Hey {{name}},</p>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    Your <a href="https://www.distrbute.com/post/{{postId}}">post</a> has been <strong>automatically approved</strong>.  
                    This happened because <strong>{{brandName}}</strong> did not take action (approve or decline) within the allowed time.
                </p>

                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    Here are the details of your auto-approved post:
                </p>

                <ul style="font-size: 16px; color: #333; margin-bottom: 25px; line-height: 1.6;">
                    <li><strong>Brand:</strong> {{brandName}}</li>
                    <li><strong>Platform:</strong> {{platform}}</li>
                    <li><strong>Content Type:</strong> {{contentType}}</li>
                    <li><strong>Approved Payout:</strong> {{amount}}</li>
                </ul>

                <div style="background-color: #e6ffed; border: 1px solid #b6f2c4; padding: 15px; border-radius: 5px; margin-bottom: 25px;">
                    <p style="font-size: 16px; color: #1b5e20; margin: 0;">
                        🙌 Great work staying active! Even though the brand didn’t respond in time,  
                        your post has been approved so you can keep moving forward and earning with <strong>Distrbute</strong>.
                    </p>
                </div>

                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    You can track all your posts and payouts anytime from your dashboard:
                </p>

                <p style="margin-bottom: 25px;">
                    👉 <a href="https://www.distrbute.com/dashboard" 
                          style="color: #2f80ed; text-decoration: none; font-weight: bold; font-size: 16px;">
                        Go to Dashboard
                    </a>
                </p>

                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 0;">
                    Keep it up,<br/>
                    The Distrbute Team
                </p>
            </div>
            
            <div style="text-align: center; margin-top: 30px; color: #666; font-size: 14px;">
                <p>© {{year}} Distrbute. All rights reserved.</p>
            </div>
        </body>
        </html>
        """;

    private static readonly string PostApproved =
        """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Post Approved</title>
        </head>
        <body style="font-family: sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;">
            <div style="max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
                
                <p style="font-size: 16px; color: #333; margin-bottom: 20px;">Hey {{name}},</p>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    🎉 Great news! Your <a href="https://www.distrbute.com/post/{{postId}}"> post </a> has been <strong>approved</strong> by <strong>{{brandName}}</strong>.
                </p>

                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    Here are the details of your approved post:
                </p>

                <ul style="font-size: 16px; color: #333; margin-bottom: 25px; line-height: 1.6;">
                    <li><strong>Brand:</strong> {{brandName}}</li>
                    <li><strong>Platform:</strong> {{platform}}</li>
                    <li><strong>Content Type:</strong> {{contentType}}</li>
                    <li><strong>Approved Payout:</strong> {{amount}}</li>
                </ul>

                <div style="background-color: #e6ffed; border: 1px solid #b6f2c4; padding: 15px; border-radius: 5px; margin-bottom: 25px;">
                    <p style="font-size: 16px; color: #1b5e20; margin: 0;">
                        🌟 Awesome job! Keep up the great work — the more quality content you share, 
                        the more opportunities you’ll unlock on <strong>Distrbute</strong>.
                    </p>
                </div>

                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    You can track your earnings and campaign progress anytime from your dashboard:
                </p>

                <p style="margin-bottom: 25px;">
                    👉 <a href="https://www.distrbute.com/dashboard" 
                          style="color: #2f80ed; text-decoration: none; font-weight: bold; font-size: 16px;">
                        Go to Dashboard
                    </a>
                </p>

                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 0;">
                    Cheers,<br/>
                    The Distrbute Team
                </p>
            </div>
            
            <div style="text-align: center; margin-top: 30px; color: #666; font-size: 14px;">
                <p>© {{year}} Distrbute. All rights reserved.</p>
            </div>
        </body>
        </html>
        """;

    private static readonly string PostSubmitted =
        """
        <!DOCTYPE html>
        <html>
        <head>
          <meta charset="UTF-8">
          <meta name="viewport" content="width=device-width, initial-scale=1.0">
          <title>New Post Submitted</title>
        </head>
        <body style="font-family: sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;">
          <div style="max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
            <p style="font-size: 16px; color: #333; margin-bottom: 20px;">Hello {{brandName}},</p>

            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
              A distributor has submitted a post for your campaign on <strong>Distrbute</strong>.
            </p>

            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
              The post is now pending your review and approval.
            </p>

            <p style="margin-bottom: 25px;">
              👉 <a href="https://www.distrbute.com/review/{{postId}}"
                    style="color: #2f80ed; text-decoration: none; font-weight: bold; font-size: 16px;">
                  Review Submission
              </a>
            </p>

            <div style="background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin-bottom: 25px;">
              <p style="font-size: 16px; color: #856404; margin: 0;">
                <strong>Important:</strong> You have <strong>{{window}}</strong> to approve or reject this post. 
                If no action is taken, it will be <strong>automatically approved</strong>.
              </p>
            </div>

            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
              Need assistance or have questions? We’re happy to help.
            </p>

            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 0;">
              Best,<br/>
              The Distrbute Team
            </p>
          </div>

          <div style="text-align: center; margin-top: 30px; color: #666; font-size: 14px;">
            <p>© {{year}} Distrbute. All rights reserved.</p>
          </div>
        </body>
        </html>
        """;

    private static readonly string PostPaid =
        """
        <!DOCTYPE html>
        <html>
        <head>
          <meta charset="UTF-8">
          <meta name="viewport" content="width=device-width, initial-scale=1.0">
          <title>You've Been Paid!</title>
        </head>
        <body style="font-family: sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;">
          <div style="max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
            <p style="font-size: 16px; color: #333; margin-bottom: 20px;">Hey {{name}},</p>

            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
              Great news! You've just been paid for your participation in a campaign on <strong>Distrbute</strong>.
            </p>

            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
              An amount of <strong>{{amount}}</strong> has been credited to your wallet.
            </p>

            <p style="margin-bottom: 25px;">
              👉 <a href="https://www.distrbute.com/wallet"
                    style="color: #27ae60; text-decoration: none; font-weight: bold; font-size: 16px;">
                  View Wallet
              </a>
            </p>

            <div style="background-color: #eafaf1; border: 1px solid #c8e6c9; padding: 15px; border-radius: 5px; margin-bottom: 25px;">
              <p style="font-size: 16px; color: #2e7d32; margin: 0;">
                <strong>Tip:</strong> Withdrawals can only be made to wallets you've linked. This helps us prevent fraud.
              </p>
            </div>

            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
              Got questions? We're here to help.
            </p>

            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 0;">
              Cheers,<br/>
              The Distrbute Team
            </p>
          </div>

          <div style="text-align: center; margin-top: 30px; color: #666; font-size: 14px;">
            <p>© {{year}} Distrbute. All rights reserved.</p>
          </div>
        </body>
        </html>
        """;

    private static readonly string CampaignInvite =
        """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            
            <title>Campaign Invitation</title>
        </head>
        <body style="font-family: sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;">
            <div style="max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
                <p style="font-size: 16px; color: #333; margin-bottom: 20px;">Hey {{name}},</p>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    Great news! A brand has invited you to collaborate on a campaign through <strong>Distrbute</strong>.
                </p>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    Tap the link below to view the invite and start earning:
                </p>
                
                <p style="margin-bottom: 25px;">
                    👉 <a href="https://www.distrbute.com/invites" 
                          style="color: #2f80ed; text-decoration: none; font-weight: bold; font-size: 16px;">
                        View Campaign Invite
                    </a>
                </p>
                
                <div style="background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin-bottom: 25px;">
                    <p style="font-size: 16px; color: #856404; margin: 0;">
                        <strong>Please note:</strong> This invite will expire in <strong>24 hours</strong>, 
                        so be sure to respond in time to secure your spot.
                    </p>
                </div>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
                    Got questions? We're here to help.
                </p>
                
                <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 0;">
                    Cheers,<br/>
                    The Distrbute Team
                </p>
            </div>
            
            <div style="text-align: center; margin-top: 30px; color: #666; font-size: 14px;">
                <p>© {{year}} Distrbute. All rights reserved.</p>
            </div>
        </body>
        </html>
        """;

    private static readonly string InvitedToCollaborate =
        """
        <!DOCTYPE html>
        <html>
        <head>
          <meta charset="UTF-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>Collaboration Invite</title>
        </head>
        <body style="font-family: sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5;">
          <div style="max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
            
            <p style="font-size: 16px; color: #333; margin-bottom: 20px;">Hey {{name}},</p>
            
            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
              <strong>{{brandName}}</strong> has invited you to join their team on <strong>Distrbute</strong> as
              <strong>{{role}}</strong>.
            </p>
            
            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
              Use the link below to preview the invite and accept your role:
            </p>
            
            <p style="margin-bottom: 25px; text-align: center;">
              <a href="https://distrbute.co/brand/invite/{{token}}" 
                 style="display: inline-block; background-color: #2f80ed; color: #ffffff; padding: 12px 20px; border-radius: 6px; text-decoration: none; font-weight: bold; font-size: 16px;">
                Preview & Accept Invite
              </a>
            </p>
            
            <div style="background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin-bottom: 25px;">
              <p style="font-size: 15px; color: #856404; margin: 0;">
                <strong>Note:</strong> This invite will expire in <strong>{{expiry}}</strong>. 
                Be sure to respond in time.
              </p>
            </div>
            
            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 20px;">
              Need help? Just reply to this email and our team will assist you.
            </p>
            
            <p style="font-size: 16px; color: #333; line-height: 1.5; margin-bottom: 0;">
              Cheers,<br/>
              The Distrbute Team
            </p>
          </div>
          
          <div style="text-align: center; margin-top: 30px; color: #666; font-size: 14px;">
            <p>© {{year}} Distrbute. All rights reserved.</p>
          </div>
        </body>
        </html>
        """;
    
    private static readonly IDictionary<string, string> Templates = new Dictionary<string, string>
    {
        ["d-12f10ee78fed4535a281b250a2b409e3"] = Otp,
        ["d-91e83c0745d748ddad0a07b70760f438"] = CampaignInvite,
        ["d-be1ad41e18784e9cb1df987f966701bb"] = PostPaid,
        ["d-d322122609504ff1b30695a789aab74b"] = PostSubmitted,
        ["d-bd2376c9ea9540b7b0265a9d9b9e3742"] = PostApproved,
        ["d-416c7284749b4242b6906f596cb01ed0"] = PostAutoApproved,
        ["d-b7dfc613d1f840c2a2fdc3dd5e86927c"] = PostDisputed,
        ["d-10a04e93e9e04b86a64cdcfc2696cb5b"] = InvitedToCollaborate
    };

    public static string GetTemplate(string templateId)
    {
        templateId = templateId.Trim();
        var template = Templates[templateId];

        return template;
    }
}