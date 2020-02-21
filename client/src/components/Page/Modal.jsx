import React, { Component } from 'react';
import PropTypes from 'prop-types';
import RBModal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

class Modal extends Component 
{

    render()
    {
        const { props } = this;

        return <RBModal show={props.show} onHide={props.onClose}>
        <RBModal.Header closeButton>
          <RBModal.Title>{props.title}</RBModal.Title>
        </RBModal.Header>
      
        <RBModal.Body>
          <p>Modal body text goes here.</p>
        </RBModal.Body>
      
        <RBModal.Footer>
          <Button variant="outline-secondary" onClick={props.onClose}>Close</Button>
          <Button variant="primary">Ok</Button>
        </RBModal.Footer>
      </RBModal>;
    }
};

Modal.defaultProps = {
  actionUrl: '',
  actionFields: [],
  title: ''
};

Modal.propTypes = {
  onClose: PropTypes.func.isRequired,
  show: PropTypes.bool.isRequired,
  actionUrl: PropTypes.string.isRequired,
  actionFields: PropTypes.arrayOf(
    PropTypes.shape({
      key: PropTypes.string.isRequired,
      fieldType: PropTypes.string.isRequired,
      label: PropTypes.string.isRequired
  })),
  title: PropTypes.string.isRequired
};

export default Modal;