#!/bin/bash

# Deploy script for OmniSuite API on AWS EC2
# This script should be run on the EC2 instance

set -e

DEPLOY_DIR="/opt/omnisuite"
CURRENT_DIR="$DEPLOY_DIR/current"
BACKUP_DIR="$DEPLOY_DIR/backup-$(date +%Y%m%d-%H%M%S)"
SERVICE_NAME="omnisuite-api"

echo "Starting deployment process..."

# Function to check if service is healthy
check_health() {
    local max_attempts=30
    local attempt=1
    
    echo "Checking service health..."
    
    while [ $attempt -le $max_attempts ]; do
        if curl -f http://localhost:5000/health 2>/dev/null || curl -f http://localhost:5000/swagger 2>/dev/null; then
            echo "Service is healthy!"
            return 0
        fi
        
        echo "Health check attempt $attempt/$max_attempts failed, waiting 10 seconds..."
        sleep 10
        ((attempt++))
    done
    
    echo "Health check failed after $max_attempts attempts"
    return 1
}

# Function to rollback deployment
rollback() {
    echo "Rolling back deployment..."
    
    # Stop current services
    cd "$CURRENT_DIR" || true
    sudo docker-compose down || true
    
    # Find the most recent backup
    local latest_backup=$(ls -t "$DEPLOY_DIR"/backup-* 2>/dev/null | head -n1)
    
    if [ -n "$latest_backup" ]; then
        echo "Rolling back to: $latest_backup"
        
        # Move current to failed
        sudo mv "$CURRENT_DIR" "$DEPLOY_DIR/failed-$(date +%Y%m%d-%H%M%S)" || true
        
        # Restore backup
        sudo mv "$latest_backup" "$CURRENT_DIR"
        
        # Start services
        cd "$CURRENT_DIR"
        sudo docker-compose up -d
        
        # Wait and check health
        sleep 30
        if check_health; then
            echo "Rollback successful!"
        else
            echo "Rollback failed!"
            exit 1
        fi
    else
        echo "No backup found for rollback!"
        exit 1
    fi
}

# Main deployment logic
main() {
    cd "$DEPLOY_DIR"
    
    # Stop existing services
    echo "Stopping existing services..."
    if [ -d "$CURRENT_DIR" ]; then
        cd "$CURRENT_DIR"
        sudo docker-compose down || true
    fi
    
    # Create backup of current deployment
    if [ -d "$CURRENT_DIR" ]; then
        echo "Creating backup..."
        sudo mv "$CURRENT_DIR" "$BACKUP_DIR"
    fi
    
    # Create new deployment directory
    echo "Creating new deployment directory..."
    sudo mkdir -p "$CURRENT_DIR"
    
    # The deployment package should already be extracted here by GitHub Actions
    # Just verify it exists
    if [ ! -f "$CURRENT_DIR/docker-compose.yml" ]; then
        echo "Deployment package not found!"
        exit 1
    fi
    
    # Set proper permissions
    echo "Setting permissions..."
    sudo chown -R $USER:$USER "$CURRENT_DIR"
    sudo chmod +x "$CURRENT_DIR"/*.sh 2>/dev/null || true
    
    # Start services
    echo "Starting services..."
    cd "$CURRENT_DIR"
    sudo docker-compose up -d --build
    
    # Wait for services to be ready
    echo "Waiting for services to start..."
    sleep 30
    
    # Health check
    if check_health; then
        echo "Deployment successful!"
        
        # Cleanup old backups (keep only last 3)
        echo "Cleaning up old backups..."
        cd "$DEPLOY_DIR"
        ls -t backup-* 2>/dev/null | tail -n +4 | sudo xargs rm -rf || true
        
        echo "Deployment completed successfully!"
    else
        echo "Health check failed, rolling back..."
        rollback
    fi
}

# Handle script arguments
case "${1:-deploy}" in
    "deploy")
        main
        ;;
    "rollback")
        rollback
        ;;
    "health")
        check_health
        ;;
    *)
        echo "Usage: $0 {deploy|rollback|health}"
        exit 1
        ;;
esac
