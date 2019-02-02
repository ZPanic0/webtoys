import React from 'react'

export default class ImageEncoder extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            imageData: '',
            file: null,
            fileName: 'Choose file...',
            fileValue: '',
            lockControls: true,
            processing: false,
            error: null,
            decodedDownloadUrl: ''
        }

        this.encodeFile = this.encodeFile.bind(this)
        this.decodeFile = this.decodeFile.bind(this)
        this.onFileInputChange = this.onFileInputChange.bind(this)
        this.onCommunicationFail = this.onCommunicationFail.bind(this)
    }

    encodeFile() {
        this.setState({
            imageData: '',
            lockControls: true,
            processing: true,
            error: null
        })
        const formData = new FormData()
        formData.append('File', this.state.file)
        fetch(staticUrls.imageEncoder.encode, { method: 'post', body: formData })
            .then(response => {
                if (!response.ok) {
                    throw response
                }
                return response.text()
            })
            .then(fileString => {
                this.setState({
                    imageData: `data:image/png;base64,${fileString}`,
                    processing: false,
                    file: null,
                    fileName: 'Choose file...'
                })
            })
            .catch(this.onCommunicationFail)
    }

    decodeFile() {
        this.setState({
            imageData: '',
            lockControls: true,
            processing: true,
            error: null
        })
        const formData = new FormData()
        formData.append('File', this.state.file)
        fetch(staticUrls.imageEncoder.decode, { method: 'post', body: formData })
            .then(response => {
                if (!response.ok) {
                    throw response
                }
                return response.json()
            })
            .then(response => {
                this.setState({
                    decodedDownloadUrl: response.url
                })
                this.downloadLink.click()
                this.setState({
                    imageData: '',
                    processing: false,
                    lockControls: false,
                    file: null,
                    fileName: 'Choose file...',
                    error: null
                })
            })
            .catch(this.onCommunicationFail)
    }

    onFileInputChange(e) {
        this.setState({
            lockControls: false,
            file: e.target.files[0],
            fileName: e.target.files[0].name,
            error: null
        })
    }

    onCommunicationFail(e) {
        this.setState({
            imageData: '',
            lockControls: false,
            processing: false,
            error: e
        })
        console.log(e)
    }

    render() {
        return (
            <div className="card card-body">
                <div className="row">
                    <div className="col-sm-6">
                        <h5 className="card-title">File To PNG Encoder/Decoder</h5>
                        <p className="card-text">A simple application to convert arbitrary files to PNG images and back.</p>
                        <div className="input-group">
                            <div className="custom-file">
                                <input onChange={this.onFileInputChange} id="fileInput" type="file" className="custom-file-input" disabled={this.state.processing} value={this.state.fileValue} />
                                <label className="custom-file-label" htmlFor="fileInput">{this.state.fileName}</label>
                            </div>
                            <div className="input-group-append">
                                <button onClick={this.encodeFile} className="btn btn-outline-secondary" disabled={this.state.lockControls ? 'disabled' : null}>Encode</button>
                                <button onClick={this.decodeFile} className="btn btn-outline-secondary" disabled={this.state.lockControls ? 'disabled' : null}>Decode</button>
                            </div>
                        </div>
                    </div>
                    <div className={this.state.imageData ? "col-sm-2" : "col-sm-6"}>
                        {this.state.processing ? <span className="fas fa-sync fa-spin"></span> : null}
                        <img className="img-fluid" src={this.state.imageData}></img>
                        <div className="alert alert-danger" style={this.state.error ? {} : { display: 'none' }} role="alert">
                            An error occurred while processing. Please check the console for more details.
                        </div>
                    </div>
                </div>
                <a ref={link => this.downloadLink = link} href={this.state.decodedDownloadUrl} style={{ display: 'none' }} download />
            </div>
        )
    }
}