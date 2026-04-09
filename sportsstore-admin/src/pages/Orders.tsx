import React, { useEffect, useState } from 'react';
import { Table, Spinner, Badge, Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { Order, orderApi } from '../api/orderApi';

const Orders: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      const data = await orderApi.getOrders();
      setOrders(data.orders);
    } catch {
      setError('Unable to connect to Order API');
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusColors: Record<string, string> = {
      Submitted: 'secondary',
      PaymentPending: 'primary',
      ShippingPending: 'info',
      ShippingCreated: 'info',
      Completed: 'success',
      Failed: 'danger'
    };
    return <Badge bg={statusColors[status] || 'secondary'}>{status}</Badge>;
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
      <h2 className="mb-4">Orders</h2>
      <Table striped bordered hover responsive>
        <thead>
          <tr>
            <th>Order ID</th>
            <th>Customer</th>
            <th>Customer ID</th>
            <th>Status</th>
            <th>Total</th>
            <th>Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {orders.length === 0 ? (
            <tr>
              <td colSpan={7} className="text-center">No orders found</td>
            </tr>
          ) : (
            orders.map(order => (
              <tr key={order.orderId}>
                <td>{order.orderId}</td>
                <td>{order.name}</td>
                <td>{order.customerId}</td>
                <td>{getStatusBadge(order.orderStatus)}</td>
                <td>${order.totalAmount.toFixed(2)}</td>
                <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                <td>
                  <Link to={`/orders/${order.orderId}`}>
                    <Button variant="primary" size="sm">View</Button>
                  </Link>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </Table>
    </div>
  );
};

export default Orders;
