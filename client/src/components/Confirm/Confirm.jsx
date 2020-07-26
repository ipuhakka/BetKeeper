import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';

class Confirm extends Component
{

  render()
  {
    const { props } = this;

    return <Modal show={props.visible} onHide={props.cancelAction}>
          <Modal.Header closeButton>
            <Modal.Title>{props.headerText}</Modal.Title>
          </Modal.Header>
        
          <Modal.Footer>
            <Button 
              variant="outline-secondary" 
              onClick={props.cancelAction}>Cancel</Button>
            <Button 
              variant={props.variant}
              onClick={props.confirmAction}>Ok</Button>
          </Modal.Footer>
        </Modal>;
  }
}

Confirm.defaultProps = {
  confirmText: 'Confirm',
  cancelText: 'Cancel',
  headerText: 'Confirm action',
  variant: 'danger'
}

Confirm.propTypes = {
  visible: PropTypes.bool.isRequired,
  confirmAction: PropTypes.func.isRequired,
  cancelAction: PropTypes.func.isRequired,
  confirmText: PropTypes.string.isRequired,
  cancelText: PropTypes.string.isRequired,
  headerText: PropTypes.string,
  variant: PropTypes.string
};

export default Confirm;
