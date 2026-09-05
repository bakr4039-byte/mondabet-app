import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:geolocator/geolocator.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';

import '../bloc/attendance_bloc.dart';
import '../bloc/attendance_event.dart';
import '../bloc/attendance_state.dart';

class CheckInScreen extends StatefulWidget {
  const CheckInScreen({super.key});

  @override
  State<CheckInScreen> createState() => _CheckInScreenState();
}

class _CheckInScreenState extends State<CheckInScreen> {
  GoogleMapController? _mapController;
  Position? _position;

  @override
  void initState() {
    super.initState();
    context.read<AttendanceBloc>().add(const AttendanceStarted());
    _startLocationUpdates();
  }

  void _startLocationUpdates() {
    Geolocator.getPositionStream(
      locationSettings: const LocationSettings(accuracy: LocationAccuracy.high),
    ).listen((pos) {
      if (!mounted) return;
      setState(() => _position = pos);
      context.read<AttendanceBloc>().add(
            LocationRefreshed(lat: pos.latitude, lng: pos.longitude),
          );
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('attendance.check_in'.tr()),
        actions: [
          BlocBuilder<AttendanceBloc, AttendanceState>(
            builder: (ctx, state) {
              final pending = state is AttendanceLoaded ? state.pendingCount : 0;
              if (pending == 0) return const SizedBox.shrink();
              return IconButton(
                tooltip: 'attendance.sync_now'.tr(),
                icon: Badge(
                  label: Text('$pending'),
                  child: const Icon(Icons.sync),
                ),
                onPressed: () => ctx.read<AttendanceBloc>().add(const SyncRequested()),
              );
            },
          ),
        ],
      ),
      body: BlocConsumer<AttendanceBloc, AttendanceState>(
        listener: (ctx, state) {
          if (state is AttendanceCheckedIn) {
            ScaffoldMessenger.of(ctx).showSnackBar(
              SnackBar(
                content: Text(
                  state.record.pendingSync
                      ? 'attendance.check_in_queued'.tr()
                      : 'attendance.check_in_success'.tr(),
                ),
              ),
            );
          } else if (state is AttendanceCheckedOut) {
            ScaffoldMessenger.of(ctx).showSnackBar(
              SnackBar(content: Text('attendance.check_out_success'.tr())),
            );
          } else if (state is AttendanceFailure) {
            ScaffoldMessenger.of(ctx).showSnackBar(
              SnackBar(content: Text(state.failure.message)),
            );
          }
        },
        builder: (ctx, state) {
          if (state is AttendanceLoading) {
            return const Center(child: CircularProgressIndicator());
          }
          if (state is AttendanceLoaded) {
            return _buildContent(ctx, state);
          }
          return const Center(child: CircularProgressIndicator());
        },
      ),
    );
  }

  Widget _buildContent(BuildContext ctx, AttendanceLoaded state) {
    final shift = state.shift;
    final pos = _position;

    return Column(
      children: [
        if (state.pendingCount > 0)
          Container(
            width: double.infinity,
            color: Colors.amber.shade100,
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            child: Row(
              children: [
                const Icon(Icons.cloud_off, size: 18, color: Colors.brown),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    'attendance.pending_sync'.tr(namedArgs: {'count': '${state.pendingCount}'}),
                    style: const TextStyle(color: Colors.brown, fontSize: 12),
                  ),
                ),
              ],
            ),
          ),
        Expanded(
          child: GoogleMap(
            initialCameraPosition: CameraPosition(
              target: shift != null
                  ? LatLng(shift.latitude, shift.longitude)
                  : const LatLng(24.7136, 46.6753),
              zoom: 15,
            ),
            onMapCreated: (c) => _mapController = c,
            markers: {
              if (shift != null)
                Marker(
                  markerId: const MarkerId('office'),
                  position: LatLng(shift.latitude, shift.longitude),
                  infoWindow: InfoWindow(title: shift.name),
                ),
              if (pos != null)
                Marker(
                  markerId: const MarkerId('me'),
                  position: LatLng(pos.latitude, pos.longitude),
                  icon: BitmapDescriptor.defaultMarkerWithHue(
                    BitmapDescriptor.hueBlue,
                  ),
                ),
            },
            circles: {
              if (shift != null)
                Circle(
                  circleId: const CircleId('geofence'),
                  center: LatLng(shift.latitude, shift.longitude),
                  radius: shift.radiusMeters.toDouble(),
                  fillColor: Colors.blue.withOpacity(0.15),
                  strokeColor: Colors.blue,
                  strokeWidth: 2,
                ),
            },
          ),
        ),
        _buildBottomPanel(ctx, state),
      ],
    );
  }

  Widget _buildBottomPanel(BuildContext ctx, AttendanceLoaded state) {
    final inZone = state.isWithinGeofence;
    final dist = state.distanceToShift;
    final isCheckedIn = state.openRecord != null;

    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (state.shift != null)
            Text(
              state.shift!.name,
              style: Theme.of(ctx).textTheme.titleMedium,
            ),
          if (dist != null && !isCheckedIn)
            Text(
              inZone
                  ? 'attendance.within_geofence'.tr()
                  : 'attendance.distance'
                      .tr(namedArgs: {'distance': dist.toInt().toString()}),
              style: TextStyle(color: inZone ? Colors.green : Colors.orange),
            ),
          const SizedBox(height: 12),
          if (isCheckedIn)
            ElevatedButton.icon(
              icon: const Icon(Icons.logout),
              label: Text('attendance.check_out'.tr()),
              style: ElevatedButton.styleFrom(backgroundColor: Colors.redAccent),
              onPressed: () => ctx.read<AttendanceBloc>().add(const CheckOutRequested()),
            )
          else
            ElevatedButton.icon(
              icon: const Icon(Icons.login),
              label: Text('attendance.check_in'.tr()),
              onPressed: inZone && _position != null
                  ? () => ctx.read<AttendanceBloc>().add(
                        CheckInRequested(
                          lat: _position!.latitude,
                          lng: _position!.longitude,
                          deviceId: 'device-001',
                        ),
                      )
                  : null,
            ),
        ],
      ),
    );
  }
}
