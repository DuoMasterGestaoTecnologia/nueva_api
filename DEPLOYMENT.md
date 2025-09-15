# Deploy Automatizado na AWS EC2

Este documento explica como configurar o deploy automatizado da OmniSuite API na AWS EC2 usando GitHub Actions.

## Pré-requisitos

1. **Instância EC2 na AWS** com:
   - Ubuntu 20.04+ ou Amazon Linux 2
   - Docker e Docker Compose instalados
   - Acesso SSH configurado
   - Portas 22, 80, 443, 5000 abertas no Security Group

2. **Usuário IAM na AWS** com permissões para:
   - Acessar a instância EC2
   - Fazer upload de arquivos via SCP

## Configuração dos Secrets do GitHub

Acesse o repositório no GitHub → Settings → Secrets and variables → Actions e adicione os seguintes secrets:

### Secrets Obrigatórios

| Secret | Descrição | Exemplo |
|--------|-----------|---------|
| `AWS_ACCESS_KEY_ID` | Access Key ID do usuário IAM | `AKIAIOSFODNN7EXAMPLE` |
| `AWS_SECRET_ACCESS_KEY` | Secret Access Key do usuário IAM | `wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY` |
| `AWS_REGION` | Região da AWS onde está a EC2 | `us-east-1` |
| `EC2_HOST` | IP público ou DNS da instância EC2 | `ec2-54-123-45-67.compute-1.amazonaws.com` |
| `EC2_USERNAME` | Usuário para conexão SSH | `ubuntu` ou `ec2-user` |
| `EC2_SSH_KEY` | Chave privada SSH para acesso à EC2 | Conteúdo completo da chave `.pem` |
| `EC2_PORT` | Porta SSH (opcional, padrão: 22) | `22` |

### Como obter a chave SSH

1. Baixe o arquivo `.pem` da sua instância EC2
2. Copie todo o conteúdo do arquivo (incluindo `-----BEGIN RSA PRIVATE KEY-----` e `-----END RSA PRIVATE KEY-----`)
3. Cole no secret `EC2_SSH_KEY`

## Configuração da Instância EC2

### 1. Preparação Inicial

Execute o script de setup na sua instância EC2:

**Para Ubuntu/Debian:**
```bash
# Conecte-se à instância EC2
ssh -i sua-chave.pem ubuntu@seu-ip-ec2

# Baixe e execute o script de setup
curl -O https://raw.githubusercontent.com/rbarins/nueva_api/main/deploy-scripts/setup-ec2.sh
chmod +x setup-ec2.sh
./setup-ec2.sh
```

**Para Amazon Linux 2023:**
```bash
# Conecte-se à instância EC2
ssh -i sua-chave.pem ec2-user@seu-ip-ec2

# Baixe e execute o script de setup
curl -O https://raw.githubusercontent.com/rbarins/nueva_api/main/deploy-scripts/setup-ec2-amazon-linux.sh
chmod +x setup-ec2-amazon-linux.sh
./setup-ec2-amazon-linux.sh
```

### 2. Configuração Manual (Alternativa)

**Para Ubuntu/Debian:**
```bash
# Instalar Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

# Instalar Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Instalar curl
sudo apt-get update && sudo apt-get install -y curl

# Criar diretório de deploy
sudo mkdir -p /opt/omnisuite
sudo chown $USER:$USER /opt/omnisuite

# Configurar firewall
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 5000/tcp
sudo ufw --force enable
```

**Para Amazon Linux 2023:**
```bash
# Atualizar sistema
sudo dnf update -y

# Instalar pacotes necessários
sudo dnf install -y curl wget git

# Instalar Docker
sudo dnf install -y docker
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -aG docker $USER

# Instalar Docker Compose
COMPOSE_VERSION=$(curl -s https://api.github.com/repos/docker/compose/releases/latest | grep 'tag_name' | cut -d\" -f4)
sudo curl -L "https://github.com/docker/compose/releases/download/${COMPOSE_VERSION}/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Criar diretório de deploy
sudo mkdir -p /opt/omnisuite
sudo chown $USER:$USER /opt/omnisuite

# Configurar firewall (firewalld)
sudo systemctl start firewalld
sudo systemctl enable firewalld
sudo firewall-cmd --permanent --add-port=22/tcp
sudo firewall-cmd --permanent --add-port=80/tcp
sudo firewall-cmd --permanent --add-port=443/tcp
sudo firewall-cmd --permanent --add-port=5000/tcp
sudo firewall-cmd --reload
```

## Como Funciona o Deploy

### Trigger do Deploy

O deploy é executado automaticamente quando:
- Um push é feito para a branch `main`
- Os testes passam com sucesso
- O build é concluído com sucesso

### Processo de Deploy

1. **Build**: A aplicação é compilada e empacotada
2. **Upload**: O pacote é enviado para a EC2 via SCP
3. **Backup**: A versão atual é movida para backup
4. **Deploy**: A nova versão é extraída e os serviços são iniciados
5. **Health Check**: Verifica se a aplicação está funcionando
6. **Rollback**: Se falhar, volta para a versão anterior automaticamente

### Estrutura de Diretórios na EC2

```
/opt/omnisuite/
├── current/           # Versão atual em produção
├── backup-YYYYMMDD-HHMMSS/  # Backups automáticos
└── failed-YYYYMMDD-HHMMSS/  # Versões que falharam
```

## Monitoramento e Gerenciamento

### Verificar Status da Aplicação

```bash
# Health check
curl http://seu-ip-ec2:5000/health

# Swagger UI
curl http://seu-ip-ec2:5000/swagger

# Logs do Docker
cd /opt/omnisuite/current
docker-compose logs -f
```

### Comandos de Gerenciamento

```bash
# Deploy manual
cd /opt/omnisuite/current
./deploy.sh deploy

# Rollback manual
cd /opt/omnisuite/current
./deploy.sh rollback

# Health check
cd /opt/omnisuite/current
./deploy.sh health

# Parar serviços
cd /opt/omnisuite/current
docker-compose down

# Iniciar serviços
cd /opt/omnisuite/current
docker-compose up -d
```

## Troubleshooting

### Problemas Comuns

1. **Falha na conexão SSH**
   - Verifique se a chave SSH está correta
   - Confirme se o Security Group permite SSH (porta 22)
   - Teste a conexão manualmente

2. **Falha no Health Check**
   - Verifique se a aplicação está rodando na porta 5000
   - Confirme se o Security Group permite tráfego na porta 5000
   - Verifique os logs: `docker-compose logs`

3. **Falha no Docker**
   - Verifique se o Docker está instalado e rodando
   - Confirme se o usuário tem permissões para usar Docker
   - Execute: `sudo systemctl status docker`

4. **Falha no Deploy**
   - Verifique os logs do GitHub Actions
   - Confirme se todos os secrets estão configurados
   - Teste o deploy manual na EC2

### Logs Importantes

```bash
# Logs do GitHub Actions
# Acesse: GitHub → Actions → [Workflow Run] → Deploy to AWS EC2

# Logs da aplicação
cd /opt/omnisuite/current
docker-compose logs omnisuite-api

# Logs do sistema
sudo journalctl -u docker
sudo journalctl -u omnisuite
```

## Segurança

### Recomendações

1. **Use IAM Roles** quando possível em vez de Access Keys
2. **Rotacione as chaves SSH** regularmente
3. **Monitore os logs** de acesso e deploy
4. **Use HTTPS** em produção (configure um Load Balancer ou reverse proxy)
5. **Mantenha a instância EC2 atualizada**

### Configuração de HTTPS (Opcional)

Para usar HTTPS em produção, configure um Load Balancer da AWS ou um reverse proxy como Nginx:

```nginx
server {
    listen 80;
    server_name seu-dominio.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## Suporte

Para problemas ou dúvidas:
1. Verifique os logs do GitHub Actions
2. Consulte os logs da aplicação na EC2
3. Teste os comandos manualmente na EC2
4. Verifique a configuração dos secrets do GitHub
