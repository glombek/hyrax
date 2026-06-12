<img src="./screaming-hyrax.svg" alt="A vector image of a screaming hyrax." align="right" width="30%" />

# hyrax

**Hy**pertext **R**esource **A**ctivity e**X**posure (that's **ActivityPub** and **RSS** exposure to you and me, as well as more [IndieWeb](https://indieweb.org/) tech going forwards) for **ASP.NET** websites.

[![Downloads](https://img.shields.io/nuget/dt/hyrax.Umbraco?color=cc9900)](https://www.nuget.org/packages/hyrax.Umbraco/)
[![NuGet](https://img.shields.io/nuget/vpre/hyrax.Umbraco?color=0273B3)](https://www.nuget.org/packages/hyrax.Umbraco)
[![GitHub license](https://img.shields.io/github/license/glombek/hyrax?color=8AB803)](../LICENSE)

The project aims to provide configuration options for **ASP.NET Core** and the **Umbraco CMS** (~12+) to expose **ActivityPub** endpoints and **RSS** feeds to expose blog content to RSS readers and the fediverse.

[Hyraxes](https://en.wikipedia.org/wiki/Hyrax) or *dassies*, although similar in appearance a marmot or particularly angry rodent, are actually part of the [paenungulata](https://en.wikipedia.org/wiki/Paenungulata) clade along with elephants and **mastodons**.

<br clear="right"/>

## Installation

Add the package to an existing Umbraco website (v10.4+) from nuget:

`dotnet add package hyrax.Umbraco`

TODO *provide any other instructions for someone using your package*

## Contributing

Contributions to this package are most welcome! Please read the [Contributing Guidelines](.github/CONTRIBUTING.md).



The test site username is `admin@example.com` and the password is `1234567890`.

## Testing with Cloudflare Tunnels and browser.pub

To test ActivityPub endpoints with [browser.pub](https://browser.pub/), you can use Cloudflare Tunnels to expose your local development server to the internet securely over HTTPS.

### Prerequisites

- [Cloudflare account](https://dash.cloudflare.com/sign-up) (free tier is sufficient)
- A domain registered with Cloudflare (or add an existing domain to Cloudflare)
- [cloudflared CLI](https://developers.cloudflare.com/cloudflare-one/connections/connect-applications/install-and-setup/) installed, or use the web-based tunnel setup

### Setup Instructions

#### Option 1: Using cloudflared CLI (Recommended)

1. **Install Cloudflare's tunneling tool `cloudflared`:**
   ```bash
   # On Windows using Chocolatey
   choco install cloudflared

   # Or download from: https://github.com/cloudflare/cloudflared/releases
   ```

2. **Authenticate with Cloudflare:**
   ```bash
   cloudflared login
   ```
   This will open a browser window to authenticate and authorize cloudflared to create tunnels.

3. **Create a tunnel:**
   ```bash
   cloudflared tunnel create hyrax-test
   ```
   This creates a persistent tunnel named "hyrax-test". Note the tunnel ID that is displayed.

4. **Configure the tunnel** by creating or editing `~/.cloudflared/config.yml`:
   ```yaml
   tunnel: hyrax-test
   credentials-file: /path/to/.cloudflared/<tunnel-id>.json

   ingress:
     - hostname: hyrax-test.yourdomain.com
       service: https://localhost:44358
     - service: http_status:404
   ```
   Replace:
   - `yourdomain.com` with your actual Cloudflare domain
   - `<tunnel-id>` with the UUID shown when you created the tunnel
   - `/path/to` with the full path to your home directory's `.cloudflared` folder

5. **Create DNS record** in the Cloudflare dashboard:
   - Go to your domain's DNS settings in the Cloudflare dashboard
   - Add a CNAME record with name `hyrax-test` pointing to `<tunnel-id>.cfargotunnel.com`
   - Alternatively, use: `cloudflared tunnel route dns hyrax-test hyrax-test.yourdomain.com`

6. **Run the tunnel:**
   ```bash
   cloudflared tunnel run hyrax-test
   ```
   Keep this terminal window open while testing. You should see output confirming the tunnel is running.

7. **In a separate terminal, run your local application:**
   ```bash
   dotnet run --project src/hyrax.TestSite
   ```

#### Option 2: Using Cloudflare Dashboard

1. Go to [Cloudflare Zero Trust Dashboard](https://one.dash.cloudflare.com/)
2. Navigate to **Networks** > **Tunnels**
3. Click **Create a tunnel** and follow the setup wizard
4. Select your operating system and follow the installation instructions
5. Configure the public hostname to route to `https://localhost:44358`
6. Create DNS records as instructed

### Testing with browser.pub

1. **Access the tunnel URL**: Navigate to `https://hyrax-test.yourdomain.com` in your browser to verify it's working
2. **Open browser.pub**: Visit [https://browser.pub/](https://browser.pub/)
3. **Enter your instance URL**: When prompted, enter `https://hyrax-test.yourdomain.com`
4. **Explore ActivityPub endpoints**: You should now be able to:
   - View your ActivityPub actor profile
   - Browse follower/following relationships
   - View your outbox and activities
   - Test ActivityPub interactions

### Useful cloudflared Commands

```bash
# List all active tunnels
cloudflared tunnel list

# View tunnel detailed information
cloudflared tunnel info hyrax-test

# Delete a tunnel
cloudflared tunnel delete hyrax-test

# Run tunnel with debug logging
cloudflared tunnel run hyrax-test --loglevel debug

# Run tunnel in background (Windows)
Start-Process cloudflared -ArgumentList "tunnel run hyrax-test"
```

### Troubleshooting

- **Certificate errors**: Ensure your local application is using valid HTTPS. Self-signed certificates are fine for development.
- **Connection refused**: Verify the application is running on port 44358 by checking the Program.cs URLs.
- **DNS not resolving**: Wait a few minutes for DNS propagation after creating CNAME records, or try `nslookup hyrax-test.yourdomain.com`.
- **Tunnel connection fails**: Run `cloudflared tunnel info hyrax-test` to check tunnel status and re-authenticate if needed with `cloudflared login`.
- **Mixed content warnings**: Ensure all resources on your application are loaded over HTTPS, not HTTP.

### Security Notes

- Tunnels created with `cloudflared login` are private and authenticated with your Cloudflare account.
- Avoid exposing sensitive data, credentials, or admin URLs through the tunnel during testing.
- For production use, consider using Cloudflare Access policies to restrict and log tunnel access.
- The credentials file (`~/.cloudflared/<tunnel-id>.json`) should be kept secure and added to `.gitignore` in any repository.

## Acknowledgments

[Matt Brailsford](https://umbracocommunity.social/@matt) and [Rachel Breeze](https://geekdom.social/@rachelbreezedev) for brainstorming, researching and pair-programming at [CODECABIN23](https://joe.gl/ombek/blog/codecabin-23/).

<small>Hyrax icon generated by AI with Recraft</small>
