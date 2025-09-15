#!/bin/bash

# Setup script for Amazon Linux 2023 EC2 instance to prepare it for OmniSuite API deployment
# Run this script once on your EC2 instance before the first deployment

set -e

echo "Setting up Amazon Linux 2023 EC2 instance for OmniSuite API deployment..."

# Update system packages
echo "Updating system packages..."
sudo dnf update -y

# Install required packages
echo "Installing required packages..."
sudo dnf install -y curl wget git

# Install Docker
echo "Installing Docker..."
if ! command -v docker &> /dev/null; then
    # Install Docker using Amazon Linux extras
    sudo dnf install -y docker
    sudo systemctl start docker
    sudo systemctl enable docker
    sudo usermod -aG docker $USER
    echo "Docker installed successfully!"
else
    echo "Docker is already installed!"
fi

# Install Docker Compose
echo "Installing Docker Compose..."
if ! command -v docker-compose &> /dev/null; then
    # Get the latest version
    COMPOSE_VERSION=$(curl -s https://api.github.com/repos/docker/compose/releases/latest | grep 'tag_name' | cut -d\" -f4)
    sudo curl -L "https://github.com/docker/compose/releases/download/${COMPOSE_VERSION}/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
    echo "Docker Compose installed successfully!"
else
    echo "Docker Compose is already installed!"
fi

# Install additional tools
echo "Installing additional tools..."
sudo dnf install -y htop iotop-ng netstat-nat

# Create deployment directory
echo "Creating deployment directory..."
sudo mkdir -p /opt/omnisuite
sudo chown $USER:$USER /opt/omnisuite

# Create systemd service for auto-start (optional)
echo "Creating systemd service..."
sudo tee /etc/systemd/system/omnisuite.service > /dev/null <<EOF
[Unit]
Description=OmniSuite API
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/opt/omnisuite/current
ExecStart=/usr/local/bin/docker-compose up -d
ExecStop=/usr/local/bin/docker-compose down
TimeoutStartSec=0

[Install]
WantedBy=multi-user.target
EOF

# Enable the service (optional - uncomment if you want auto-start)
# sudo systemctl enable omnisuite.service

# Configure firewall (using firewalld on Amazon Linux)
echo "Configuring firewall..."
sudo systemctl start firewalld
sudo systemctl enable firewalld

# Open required ports
sudo firewall-cmd --permanent --add-port=22/tcp    # SSH
sudo firewall-cmd --permanent --add-port=80/tcp    # HTTP
sudo firewall-cmd --permanent --add-port=443/tcp   # HTTPS
sudo firewall-cmd --permanent --add-port=5000/tcp  # API port
sudo firewall-cmd --reload

# Create log rotation configuration
echo "Setting up log rotation..."
sudo tee /etc/logrotate.d/omnisuite > /dev/null <<EOF
/opt/omnisuite/*/logs/*.log {
    daily
    missingok
    rotate 7
    compress
    delaycompress
    notifempty
    create 644 $USER $USER
    postrotate
        /usr/local/bin/docker-compose -f /opt/omnisuite/current/docker-compose.yml restart > /dev/null 2>&1 || true
    endscript
}
EOF

# Create a simple health check script
echo "Creating health check script..."
sudo tee /opt/omnisuite/health-check.sh > /dev/null <<'EOF'
#!/bin/bash
# Simple health check script for OmniSuite API

API_URL="http://localhost:5000"
MAX_ATTEMPTS=30
ATTEMPT=1

echo "Checking OmniSuite API health..."

while [ $ATTEMPT -le $MAX_ATTEMPTS ]; do
    if curl -f "$API_URL/health" 2>/dev/null || curl -f "$API_URL/swagger" 2>/dev/null; then
        echo "✅ API is healthy!"
        exit 0
    fi
    
    echo "⏳ Health check attempt $ATTEMPT/$MAX_ATTEMPTS failed, waiting 10 seconds..."
    sleep 10
    ((ATTEMPT++))
done

echo "❌ Health check failed after $MAX_ATTEMPTS attempts"
exit 1
EOF

sudo chmod +x /opt/omnisuite/health-check.sh
sudo chown $USER:$USER /opt/omnisuite/health-check.sh

# Verify installations
echo "Verifying installations..."
echo "Docker version:"
docker --version

echo "Docker Compose version:"
docker-compose --version

echo "Firewall status:"
sudo firewall-cmd --list-ports

echo ""
echo "🎉 Amazon Linux 2023 setup completed successfully!"
echo ""
echo "Next steps:"
echo "1. Log out and log back in to apply Docker group permissions"
echo "2. Configure GitHub Secrets with your AWS and EC2 credentials"
echo "3. Push to main branch to trigger deployment"
echo "4. Monitor deployment logs in GitHub Actions"
echo ""
echo "To manually test Docker:"
echo "- Test Docker: docker run hello-world"
echo "- Test Docker Compose: docker-compose --version"
echo ""
echo "To manually deploy or manage the service:"
echo "- Deploy: /opt/omnisuite/current/deploy.sh deploy"
echo "- Rollback: /opt/omnisuite/current/deploy.sh rollback"
echo "- Health check: /opt/omnisuite/health-check.sh"
echo ""
echo "⚠️  IMPORTANT: You need to log out and log back in for Docker group permissions to take effect!"