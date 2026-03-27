import 'package:easy_localization/easy_localization.dart';
import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../domain/entities/leave_request.dart';
import '../bloc/leave_bloc.dart';
import '../bloc/leave_event.dart';
import '../bloc/leave_state.dart';

class ExcuseFormScreen extends StatefulWidget {
  const ExcuseFormScreen({super.key});

  @override
  State<ExcuseFormScreen> createState() => _ExcuseFormScreenState();
}

class _ExcuseFormScreenState extends State<ExcuseFormScreen> {
  final _formKey = GlobalKey<FormState>();
  DateTime? _fromDate;
  DateTime? _toDate;
  final _reasonCtrl = TextEditingController();
  String? _filePath;
  String? _fileName;

  @override
  void dispose() {
    _reasonCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickDate(bool isFrom) async {
    final picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now().subtract(const Duration(days: 1)),
      firstDate: DateTime.now().subtract(const Duration(days: 30)),
      lastDate: DateTime.now(),
    );
    if (picked == null) return;
    setState(() {
      if (isFrom) _fromDate = picked;
      else _toDate = picked;
    });
  }

  Future<void> _pickFile() async {
    final result = await FilePicker.platform.pickFiles(
      type: FileType.custom,
      allowedExtensions: ['pdf', 'jpg', 'jpeg', 'png'],
    );
    if (result != null && result.files.single.path != null) {
      setState(() {
        _filePath = result.files.single.path!;
        _fileName = result.files.single.name;
      });
    }
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    if (_fromDate == null || _toDate == null) return;

    context.read<LeaveBloc>().add(LeaveSubmitted(
          type: LeaveType.excuse,
          fromDate: _fromDate!,
          toDate: _toDate!,
          reason: _reasonCtrl.text.isEmpty ? null : _reasonCtrl.text,
          filePath: _filePath,
        ));
  }

  @override
  Widget build(BuildContext context) {
    final fmt = DateFormat('yyyy-MM-dd');

    return Scaffold(
      appBar: AppBar(title: Text('leave.excuse'.tr())),
      body: BlocListener<LeaveBloc, LeaveState>(
        listener: (ctx, state) {
          if (state is LeaveSubmitSuccess) {
            ScaffoldMessenger.of(ctx).showSnackBar(
              SnackBar(content: Text('leave.submitted_success'.tr())),
            );
            ctx.pop();
          } else if (state is LeaveFailure) {
            ScaffoldMessenger.of(ctx).showSnackBar(
              SnackBar(content: Text(state.failure.message)),
            );
          }
        },
        child: Form(
          key: _formKey,
          child: ListView(
            padding: const EdgeInsets.all(16),
            children: [
              InkWell(
                onTap: () => _pickDate(true),
                child: InputDecorator(
                  decoration: InputDecoration(
                    labelText: 'leave.from_date'.tr(),
                    border: const OutlineInputBorder(),
                    suffixIcon: const Icon(Icons.calendar_today),
                  ),
                  child: Text(_fromDate != null ? fmt.format(_fromDate!) : '—'),
                ),
              ),
              const SizedBox(height: 12),
              InkWell(
                onTap: () => _pickDate(false),
                child: InputDecorator(
                  decoration: InputDecoration(
                    labelText: 'leave.to_date'.tr(),
                    border: const OutlineInputBorder(),
                    suffixIcon: const Icon(Icons.calendar_today),
                  ),
                  child: Text(_toDate != null ? fmt.format(_toDate!) : '—'),
                ),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _reasonCtrl,
                maxLines: 3,
                validator: (v) =>
                    v == null || v.isEmpty ? 'leave.reason_required'.tr() : null,
                decoration: InputDecoration(
                  labelText: 'leave.reason'.tr(),
                  border: const OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 12),
              OutlinedButton.icon(
                icon: const Icon(Icons.attach_file),
                label: Text(_fileName ?? 'leave.upload_file'.tr()),
                onPressed: _pickFile,
              ),
              const SizedBox(height: 24),
              BlocBuilder<LeaveBloc, LeaveState>(
                builder: (_, state) => ElevatedButton(
                  onPressed: state is LeaveLoading ? null : _submit,
                  child: state is LeaveLoading
                      ? const CircularProgressIndicator()
                      : Text('common.submit'.tr()),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
