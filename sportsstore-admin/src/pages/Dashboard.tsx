import React, { useEffect, useState } from 'react';
import { Card, Row, Col, Spinner } from 'react-bootstrap';
import { DashboardSummary, orderApi } from '../api/orderApi';

const Dashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    try {
      const data = await orderApi.getDashboardSummary();
      setStats(data);
    } catch {
      setError('Unable to connect to Order API. Make sure OrderApi is running.');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="text-center mt-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </div>
    );
  }

  if (error) {
    return (
      <div className="alert alert-warning" role="alert">
        {error}
      </div>
    );
  }

  return (
    <div>
      <h2 className="mb-4">Dashboard</h2>
      <Row className="mb-4">
        <Col md={4}>
          <Card bg="primary" text="white">
            <Card.Body>
              <Card.Title>Total Orders</Card.Title>
              <h1>{stats?.totalOrders || 0}</h1>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card bg="warning" text="dark">
            <Card.Body>
              <Card.Title>Pending</Card.Title>
              <h1>{stats?.pendingOrders || 0}</h1>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
              <Card bg="info" text="white">
            <Card.Body>
              <Card.Title>Processing</Card.Title>
              <h1>{(stats?.ordersByStatus?.ShippingCreated || 0) + (stats?.ordersByStatus?.ShippingPending || 0) + (stats?.ordersByStatus?.PaymentPending || 0)}</h1>
            </Card.Body>
          </Card>
        </Col>
      </Row>
      <Row className="mb-4">
        <Col md={4}>
          <Card bg="success" text="white">
            <Card.Body>
              <Card.Title>Completed</Card.Title>
              <h1>{stats?.completedOrders || 0}</h1>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card bg="danger" text="white">
            <Card.Body>
              <Card.Title>Failed</Card.Title>
              <h1>{stats?.failedOrders || 0}</h1>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card bg="secondary" text="white">
            <Card.Body>
              <Card.Title>Total Revenue</Card.Title>
              <h1>${((stats?.totalRevenue || 0)).toFixed(2)}</h1>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default Dashboard;
