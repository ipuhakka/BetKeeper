import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import RBModal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import Input from './Input';

/** Action modal. */
class Modal extends Component 
{
    constructor(props)
    {
      super(props);

      this.state = {
        actionResponseValues: {}
      }

      this.onChangeInputValue = this.onChangeInputValue.bind(this);
    }

    onChangeInputValue(key, newValue)
    {
      const actionResponse = {...this.state.actionResponseValues};

      actionResponse[key] = newValue;

      console.log(actionResponse);
      this.setState({
        actionResponseValues: actionResponse
      });
    }

    renderFields()
    {
      const { actionFields } = this.props;

      return _.map(actionFields, field => {
        return <Input 
          onChange={this.onChangeInputValue}
          type={field.fieldType} 
          label={field.label}
          fieldKey={field.key}
          key={field.key} />
      });
    }

    render()
    {
        const { props } = this;

        return <RBModal show={props.show} onHide={props.onClose}>
          <RBModal.Header closeButton>
            <RBModal.Title>{props.title}</RBModal.Title>
          </RBModal.Header>
        
          <RBModal.Body>
            <div>
              {this.renderFields()}
            </div>
          </RBModal.Body>
        
          <RBModal.Footer>
            <Button variant="outline-secondary" onClick={props.onClose}>Close</Button>
            <Button variant="outline-primary">Ok</Button>
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