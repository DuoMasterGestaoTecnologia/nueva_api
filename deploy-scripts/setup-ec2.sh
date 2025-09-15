#!/bin/bash

# Setup script for EC2 instance to prepare it for OmniSuite API deployment
# Run this script once on your EC2 instance before the first deployment

set -e

echo "Setting up EC2 instance for OmniSuite API deployment..."

# Update system packages
echo "Updating system packages..."
sudo apt-get update
sudo apt-get upgrade -y

# Install Docker
echo "Installing Docker..."
if ! command -v docker &> /dev/null; then
    curl -fsSL https://get.docker.com -o get-docker.sh
    sudo sh get-docker.sh
    sudo usermod -aG docker $USER
    rm get-docker.sh
fi

# Install Docker Compose
echo "Installing Docker Compose..."
if ! command -v docker-compose &> /dev/null; then
    sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
fi

# Install curl (for health checks)
echo "Installing curl..."
sudo apt-get install -y curl

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

# Configure firewall (adjust ports as needed)
echo "Configuring firewall..."
sudo ufw allow 22/tcp   # SSH
sudo ufw allow 80/tcp   # HTTP
sudo ufw allow 443/tcp  # HTTPS
sudo ufw allow 5000/tcp # API port
sudo ufw --force enable

# Install monitoring tools (optional)
echo "Installing monitoring tools..."
sudo apt-get install -y htop iotop nethogs

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

echo "EC2 setup completed successfully!"
echo ""
echo "Next steps:"
echo "1. Configure GitHub Secrets with your AWS and EC2 credentials"
echo "2. Push to main branch to trigger deployment"
echo "3. Monitor deployment logs in GitHub Actions"
echo ""
echo "To manually deploy or manage the service:"
echo "- Deploy: /opt/omnisuite/current/deploy.sh deploy"
echo "- Rollback: /opt/omnisuite/current/deploy.sh rollback"
echo "- Health check: /opt/omnisuite/current/deploy.sh health"
