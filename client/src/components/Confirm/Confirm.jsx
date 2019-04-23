import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';

class Confirm extends Component{

  render(){

    const { props } = this;

    if (props.visible){

      return (
          <Alert bsStyle={props.bsStyle}>
            <h4>{props.headerText}</h4>
            <p>
              <Button onClick={props.cancelAction} bsStyle="danger">{props.cancelText}</Button>
              <Button onClick={props.confirmAction} bsStyle="success">{props.confirmText}
              </Button>
            </p>
          </Alert>
        );
    }

    return (<div/>);
  }
}

Confirm.defaultProps = {
  confirmText: 'Confirm',
  cancelText: 'Cancel',
  headerText: 'Confirm action',
  bsStyle: 'danger'
}

Confirm.propTypes = {
  visible: PropTypes.bool.isRequired,
  confirmAction: PropTypes.func.isRequired,
  cancelAction: PropTypes.func.isRequired,
  confirmText: PropTypes.string,
  cancelText: PropTypes.string,
  headerText: PropTypes.string,
  bsStyle: PropTypes.string
};

export default Confirm;
