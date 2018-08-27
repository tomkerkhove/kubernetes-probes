# Kubernetes Probes
Experimental repo for testing the Kubernetes pod probes for liveness & readiness over TCP.

# Deployment template
```yaml
apiVersion: extensions/v1beta1
kind: Deployment
metadata:
  name: kubernetes-liveness
spec:
  replicas: 1
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 1
  template:
    metadata:
      name: kubernetes-liveness-replica-set
      labels:
        app: kubernetes-liveness
        type: app
    spec:
      containers:
      - name: kubernetes-liveness-service
        image: tomkerkhove/kubernetes-liveness-service
        env:
        - name:  Probe_Tcp_Port
          value: "8888"
        ports:
        - name: liveness-port
          containerPort: 8888
        livenessProbe:
          tcpSocket:
            port: 8888
          initialDelaySeconds: 30
          timeoutSeconds: 10
          periodSeconds: 20
```