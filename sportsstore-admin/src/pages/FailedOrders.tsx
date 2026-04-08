import React, { useEffect, useState } from 'react';
import { Table, Spinner, Badge, Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';

interface FailedOrder {
  id: number;
  customerName: string;
  customerEmail: string;
  status: string;
  totalAmount: number;
  failureReason: string;
  createdAt: string;
}

const FailedOrders: React.FC = () => {
  const [orders, setOrders] = useState<FailedOrder[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchFailedOrders();
  }, []);

  const fetchFailedOrders = async () => {
    try {
      const response = await fetch('http://localhost:5000/api/orders/status/failed');
      if (!response.ok) throw new Error('Failed to fetch failed orders');
      const data = await response.json();
      setOrders(data);
    } catch (err) {
      setError('Unable to connect to Order API');
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
      <h2 className="mb-4">Failed Orders</h2>
      {orders.length === 0 ? (
        <div className="alert alert-success">
          No failed orders found. Great news!
        </div>
      ) : (
        <Table striped bordered hover responsive>
          <thead>
            <tr>
              <th>Order ID</th>
              <th>Customer</th>
              <th>Email</th>
              <th>Total</th>
              <th>Failure Reason</th>
              <th>Date</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {orders.map(order => (
              <tr key={order.id}>
                <td>{order.id}</td>
                <td>{order.customerName}</td>
                <td>{order.customerEmail}</td>
                <td>${order.totalAmount.toFixed(2)}</td>
                <td>
                  <Badge bg="danger">{order.failureReason || 'Unknown error'}</Badge>
                </td>
                <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                <td>
                  <Link to={`/orders/${order.id}`}>
                    <Button variant="primary" size="sm">View Details</Button>
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </div>
  );
};

export default FailedOrders;
